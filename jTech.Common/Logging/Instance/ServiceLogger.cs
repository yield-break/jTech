using System;
using System.IO;

namespace jTech.Common.Logging.Instance
{
    public class ServiceLogger : Logger
    {
        private static readonly Lazy<string> _logFilePath = 
            new Lazy<string>(() => Path.Combine(Environment.CurrentDirectory, 
                                   Path.Combine("Log", String.Format("Log.{0}.txt", DateTime.Now.ToString("yyyy-MM-dd")))));

        private readonly ConsoleLogger _consoleLogger;
        private readonly FileLogger _fileLogger;

        static ServiceLogger()
        {
            var logFileDir = Path.GetDirectoryName(_logFilePath.Value);
            if (!Directory.Exists(logFileDir))
            {
                Directory.CreateDirectory(logFileDir);
            }
        }

        public ServiceLogger(Type caller) : base(caller) 
        {
            _consoleLogger = new ConsoleLogger(caller);
            _fileLogger = new FileLogger(caller, _logFilePath.Value);
        }

        protected internal override void DoLog(LogCatagory catagory, string message)
        {
            if (Environment.UserInteractive)
            {
                _consoleLogger.DoLog(catagory, message);
            }

            if (catagory == LogCatagory.Debug) return; // TODO: Take this from configuration, rather than hard code.
            _fileLogger.DoLog(catagory, message);
        }

    }
}
