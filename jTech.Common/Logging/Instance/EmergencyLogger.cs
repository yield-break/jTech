using jTech.Common.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jTech.Common.Logging.Instance
{
    // Lazy Singleton pattern.
    // EmergencyLogger is to be used in situations where a standard logger cannot be instantiated and a last ditch log
    // out is required before terminating an application.
    public class EmergencyLogger : Logger
    {
        private static readonly Lazy<EmergencyLogger> _lazyInstance = new Lazy<EmergencyLogger>(() => new EmergencyLogger());

        private readonly string _logFilePath;
        private readonly IFileWriter _fileWriter;
        private readonly EventLog _eventLog;

        private EmergencyLogger() : base(typeof(EmergencyLogger))
        {
            _logFilePath = Path.Combine(Environment.CurrentDirectory, String.Format("EmergencyLog.{0}.txt", DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.ff")));
            _fileWriter = new FileWriter();

            string SourceName = "WindowsService.ExceptionLog";
            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, "Application");
            }

            _eventLog = new EventLog();
            _eventLog.Source = SourceName;
        }

        public static EmergencyLogger Instance { get { return _lazyInstance.Value; } }

        protected internal override void DoLog(LogCatagory catagory, string message)
        {
            _eventLog.WriteEntry(message, GetLogType(catagory));
            _fileWriter.Write(_logFilePath, message);
        }

        private static EventLogEntryType GetLogType(LogCatagory category)
        {
            switch (category)
            {
                case LogCatagory.Unknown:
                    return EventLogEntryType.Error;
                case LogCatagory.Debug:
                    return EventLogEntryType.Information;
                case LogCatagory.Info:
                    return EventLogEntryType.Information;
                case LogCatagory.Warn:
                    return EventLogEntryType.Warning;
                case LogCatagory.Error:
                    return EventLogEntryType.Error;
                default:
                    return EventLogEntryType.Error;
            }
        }

    }
}
