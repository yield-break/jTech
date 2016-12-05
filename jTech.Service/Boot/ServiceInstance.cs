using System;
using System.ComponentModel.Composition;
using System.ServiceProcess;
using System.Threading;
using jTech.Common.Core;
using jTech.Common.Logging;

namespace jTech.Service.Boot
{
    public interface IServiceInstance
    {
        ManualResetEvent InstanceStopped { get; }

        void StartInstance(); 
        void StopInstance();
    }

    [System.ComponentModel.DesignerCategory("Code")]
    [Export(typeof(IServiceInstance))]
    public class ServiceInstance : ServiceBase, IServiceInstance
    {
        private const string ServiceNameKey = "ServiceName";

        private readonly ILogger _log;
        private readonly IServiceRunner _serviceRunner;

        public ServiceInstance() { }
        [ImportingConstructor]
        public ServiceInstance
        (
            ILogFactory logFactory,
            IConfiguration configuration,
            IServiceRunner serviceRunner
        )
        {
            try
            {
                _log = logFactory.GetLogger(this);
                _serviceRunner = serviceRunner;

                //this.ServiceName = configuration[ServiceNameKey];

                InstanceStopped = new ManualResetEvent(false);

                InitializeComponent();
            }
            catch (Exception e)
            {
                _log.Log(LogCatagory.Error, e, "Service Instance caught an unexpected exception on initialise.");
                throw;
            }
        }

        public ManualResetEvent InstanceStopped { get; private set; }

        protected override void OnStart(string[] args)
        {
            StartInstance();
        }

        protected override void OnStop()
        {
            StopInstance();
        }

        protected override void OnShutdown()
        {
 	         StopInstance();
        }

        public void StartInstance()
        {
            try
            {
                _log.Log(LogCatagory.Info, "Starting service runner...");
                _serviceRunner.Run();
                _log.Log(LogCatagory.Info, "Service runner started");
            }
            catch (Exception e)
            {
                _log.Log(LogCatagory.Error, e, String.Format("ServiceInstance caught an unexpected exception on start."));
            }
        }

        public void StopInstance()
        {
            try
            {
                _log.Log(LogCatagory.Info, "Service runner stopping...");
                _serviceRunner.GracefulShutdown();
                _log.Log(LogCatagory.Info, "Service runner stopped.");
            }
            catch (Exception e)
            {
                _log.Log(LogCatagory.Error, e, String.Format("ServiceInstance caught an unexpected exception on stop."));
            }
            finally
            {
                InstanceStopped.Set();
            }
        }

        private void InitializeComponent()
        {
            // 
            // ServiceInstance
            // 

        }

    }
}
