using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using jTech.Common.Core;
using jTech.Common.Logging;
using jTech.Common.Logging.Instance;

namespace jTech.Service.Boot
{
    [System.ComponentModel.DesignerCategory("Code")]
    public partial class ServiceInstaller : System.Configuration.Install.Installer
    {
        protected void Install(ServiceConfiguration configuration)
        {
            try
            {
                var serviceInstaller = new System.ServiceProcess.ServiceInstaller
                {
                    DelayedAutoStart = configuration.DelayedAutoStart,
                    Description = configuration.Description,
                    DisplayName = configuration.DisplayName,
                    ServiceName = configuration.ServiceName,
                    ServicesDependedOn = configuration.ServicesDependedOn,
                    StartType = configuration.StartType,
                };

                var serviceProcessInstaller = new ServiceProcessInstaller
                {
                    Account = configuration.Account,
                    Username = configuration.Username,
                    Password = configuration.Password,
                };

                Installers.Add(serviceInstaller);
                Installers.Add(serviceProcessInstaller);
            }
            catch (Exception e)
            {
                ILogger emergencyLogger = EmergencyLogger.Instance;
                emergencyLogger.Log(LogCatagory.Error, e, "ServiceInstaller caught an unexpected exception while attempting to create installers.");
                throw;
            }
        }
        
    }
}
