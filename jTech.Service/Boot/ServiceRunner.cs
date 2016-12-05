using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using jTech.Common.Logging;
using jTech.Common.Reactive;
using jTech.Common.Reactive.Events;
using jTech.Service.ServiceCommunication;

namespace jTech.Service.Boot
{
    public interface IServiceRunner
    {
        void Run();
        void GracefulShutdown();
    }

    [Export(typeof(IServiceRunner))]
    public class ServiceRunner : IServiceRunner
    {
        private readonly ILogger _log;
        private readonly IPeriodicProcessScheduler _periodicProcessScheduler;
        private readonly IEventPublisher<IServiceRequest> _serviceEventPublisher;
        private readonly List<IjTechService> _services;

        [ImportingConstructor]
        public ServiceRunner
        (
            ILogFactory logFactory,
            IPeriodicProcessScheduler periodicProcessScheduler,
            IEventPublisher<IServiceRequest> serviceEventPublisher,
            [ImportMany] IEnumerable<IjTechService> services
        )
        {
            _log = logFactory.GetLogger(this);
            _periodicProcessScheduler = periodicProcessScheduler;
            _serviceEventPublisher = serviceEventPublisher;
            _services = services.ToList();
        }

        public void Run()
        {
            try
            {
                if (!_services.Any())
                {
                    _log.Log(LogCatagory.Warn, "ServiceRunner did not find any instances of IjTechService.");
                    return;
                }

                foreach(IjTechService service in _services)
                {
                    _log.Log(LogCatagory.Info, "Starting {0}...", service);

                    service.Start();

                    _log.Log(LogCatagory.Info, "Started {0}.", service);
                }

                _log.Log(LogCatagory.Info, "Scheduling day roll.");

                var midnight = DateTime.Today
                                        .AddDays(1)
                                        .AddSeconds(10); // 10 seconds past midnight.

                _periodicProcessScheduler.ScheduleProcessAsync(
                    OnDayRoll, 
                    new CancellationTokenSource().Token, 
                    TimeSpan.FromDays(1),
                    midnight - DateTime.Now);

                _log.Log(LogCatagory.Info, "Day roll scheduled. Next run at {0}.", midnight.ToString("yyyy-MM-dd HH:mm:ss.ff"));

            }
            catch (Exception e)
            {
                _log.Log(LogCatagory.Error, e, "ServiceRunner caught an unexpected exception while attempting to run service.");
            }
        }

        public void GracefulShutdown()
        {
            try
            {
                foreach(IjTechService service in _services)
                {
                    _log.Log(LogCatagory.Info, "Stopping {0}..", service);

                    try
                    {
                        service.Stop();
                    }
                    catch (Exception e)
                    {
                        _log.Log(LogCatagory.Error, e, "ServiceRunner caught an unexpected exception while attempting stop {0}.", service);
                    }

                    _log.Log(LogCatagory.Info, "Stopped {0}.", service);
                }
            }
            catch (Exception e)
            {
                _log.Log(LogCatagory.Error, e, "ServiceRunner caught an unexpected exception while attempting to gracefully shutdown service.");
            }
        }

        private void OnDayRoll(object sender, EventArgs args)
        {
            _log.Log(LogCatagory.Info, "Publishing day roll event.");
            _serviceEventPublisher.Publish(new DayRollRequest(this, String.Format("The time is {0} and it's a new day.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"))));
        }

    }
}
