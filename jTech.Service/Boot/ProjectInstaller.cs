using System.Configuration.Install;
using jTech.Common.Logging;
using jTech.Common.Logging.Instance;
using System;

namespace jTech.Service.Boot
{
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected void Install(string serviceName)
        {
            try
            {
                var serviceInstaller = new System.ServiceProcess.ServiceInstaller();
                serviceInstaller.ServiceName = serviceName;

                Installers.Add(serviceInstaller);
            }
            catch (Exception e)
            {
                ILogger emergencyLogger = EmergencyLogger.Instance;
                emergencyLogger.Log(LogCatagory.Error, e, "ProjectInstaller caught an unexpected exception while attempting to create installers.");
                throw;
            }
        }

    }
}
