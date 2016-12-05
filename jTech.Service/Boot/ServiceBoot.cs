using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Threading;
using jTech.Common.Logging;
using jTech.Common.Reactive;
using jTech.Common.Reactive.Events;
using jTech.Service.ServiceCommunication;

namespace jTech.Service.Boot
{
    public interface IServiceBoot
    {
        void Boot();
        void Boot(IServiceArgs serviceArgs);
    }

    [Export(typeof(IServiceBoot))]
    public class ServiceBoot : IServiceBoot
    {
        private readonly ILogger _log;
        private readonly IServiceInstance _serviceInstance;

        [ImportingConstructor]
        public ServiceBoot
        (
            ILogFactory logFactory,
            IServiceInstance serviceInstance,
            IEventPublisher<IServiceRequest> serviceEventPublisher
        )
        {
            _log = logFactory.GetLogger(this);
            _serviceInstance = serviceInstance;
            serviceEventPublisher.GetEvent<GracefulShutdownRequest>()
                                 .Take(1) // One is enough.
                                 .Subscribe(OnGracefulShutdownRequest);
        }

        public void Boot()
        {
            Boot(null);
        }
        public void Boot(IServiceArgs serviceArgs)
        {
            if (serviceArgs != null)
            {
                if (serviceArgs.LaunchDebugger)
                {
                    Debugger.Launch();
                }
            }

            try
            {
                _log.Log(LogCatagory.Info, "ServiceBoot starting Windows service instance.");

                // Entry point when running in console mode.
                if (Environment.UserInteractive)
                {
                    SubscribeToConsoleCancel();

                    _serviceInstance.StartInstance();

                    _log.Log(LogCatagory.Info, "Windows service instance started. Waiting for stop signal.");
                    // Wait on the stop signal of all instances of ServiceInstance. This will simulate the behaviour of ServiceBase.Run().
                    _serviceInstance.InstanceStopped.WaitOne();
                }
                // Entry point when running as a service.
                else
                {
                    var service = _serviceInstance as ServiceBase;
                    if (service == null)
                    {
                        _log.Log(LogCatagory.Warn, String.Format("Could not cast service instnace to type of ServiceBase. Instance: {0}.", _serviceInstance));
                    }

                    ServiceBase.Run(service);
                }

                _log.Log(LogCatagory.Info, "Windows service instance has stopped.");
            }
            catch (Exception e)
            {
                _log.Log(LogCatagory.Error, e, "ServiceBoot caught an unexpected exception while in Boot().");
            }
        }

        private void OnGracefulShutdownRequest(GracefulShutdownRequest request)
        {
            try
            {
                _log.Log(LogCatagory.Info, "ServiceBoot recieved request from {0} to gracefully shutdown service. Reason: {1}", request.Sender, request.Reason);

                _log.Log(LogCatagory.Info, "Stopping Windows service instance...");
                if (Environment.UserInteractive)
                {
                    _serviceInstance.StopInstance();
                }
                else
                {
                    var service = _serviceInstance as ServiceBase;
                    if (service == null)
                    {
                        _log.Log(LogCatagory.Warn, String.Format("Could not cast service instnace to type of ServiceBase. Instance: {0}.", _serviceInstance));
                    }

                    service.Stop();
                }
                _log.Log(LogCatagory.Info, "Completed graceful shutdown of service.");

            }
            catch (Exception e)
            {
                _log.Log(LogCatagory.Error, e, "ServiceBoot caught an unexpected exception while attempting to gracefully shutdown services.");
            }
        }

        private void SubscribeToConsoleCancel()
        {
            Console.CancelKeyPress += OnConsoleCancel;
        }

        private void OnConsoleCancel(object sender, ConsoleCancelEventArgs args)
        {
            string reason = "ServiceBoot recieved notification of console cancel key press.";
            _log.Log(LogCatagory.Info, reason);
            
            // Cancel event to prevent our app domain being killed before we can gracefully stop our processes.
            args.Cancel = true;

            OnGracefulShutdownRequest(new GracefulShutdownRequest(this, reason));
        }

    }
}
