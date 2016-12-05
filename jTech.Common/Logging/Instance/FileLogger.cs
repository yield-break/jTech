using System;
using System.IO;
using jTech.Common.IO;
using jTech.Common.Reactive;
using jTech.Common.Reactive.Events;

namespace jTech.Common.Logging.Instance
{
    public class FileLogger : Logger
    {
        private readonly IFileWriter _fileWriter;
        private readonly Logger _logger;

        private string _filePath;

        public FileLogger(Type caller) : this(caller, logger: null) { }
        public FileLogger
        (
            Type caller, 
            Logger logger
        ) 
            : base(caller)
        {
            _logger = logger;
        }

        public FileLogger(Type caller, IEventPublisher<IServiceRequest> eventPublisher) : this(caller, eventPublisher, null) { }
        public FileLogger
        (
            Type caller,
            IEventPublisher<IServiceRequest> eventPublisher,
            Logger logger
        )
            : base(caller, eventPublisher) 
        {
            _fileWriter = new FileWriter();
            _logger = logger;
            _filePath = GetLogFilePath();
        }

        protected internal override void DoLog(LogCatagory catagory, string message)
        {
            if (catagory != LogCatagory.Debug) // TODO: Take this from configuration, rather than hard code.
            {
                _fileWriter.Write(_filePath, message);
            }

            if (_logger != null) _logger.DoLog(catagory, message);
        }

        protected internal override void OnDayRoll()
        {
            // On day roll, get today's log file path.
            _filePath = GetLogFilePath();
            
            if (_logger != null) _logger.OnDayRoll();
        }

        private static string GetLogFilePath()
        {
            var logFileDir = Path.Combine(Environment.CurrentDirectory, "Log");
            if (!Directory.Exists(logFileDir))
            {
                Directory.CreateDirectory(logFileDir);
            }

            string logFilePath = Path.Combine(logFileDir, String.Format("Log.{0}.txt", DateTime.Now.ToString("yyyy-MM-dd")));

            return logFilePath;
        }

    }
}
