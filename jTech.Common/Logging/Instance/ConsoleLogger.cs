using System;
using jTech.Common.Reactive;
using jTech.Common.Reactive.Events;

namespace jTech.Common.Logging.Instance
{
    public class ConsoleLogger : Logger
    {
        private static readonly object _consoleLock = new object();

        private readonly Logger _logger;

        public ConsoleLogger(Type caller) : this (caller, logger: null) { }
        public ConsoleLogger
        (
            Type caller, 
            Logger logger
        ) 
            : base(caller)
        {
            _logger = logger;
        }

        public ConsoleLogger(Type caller, IEventPublisher<IServiceRequest> eventPublisher) : this(caller, eventPublisher, null) { }
        public ConsoleLogger
        (
            Type caller,
            IEventPublisher<IServiceRequest> eventPublisher,
            Logger logger
        )
            : base(caller, eventPublisher)
        {
            _logger = logger;
        }

        protected internal override void DoLog(LogCatagory catagory, string message)
        {
            if (Environment.UserInteractive)
            {
                lock (_consoleLock)
                {
                    ConsoleColor defaultColour = Console.ForegroundColor;

                    switch (catagory)
                    {
                        case LogCatagory.Debug:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case LogCatagory.Warn:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case LogCatagory.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                    }

                    Console.WriteLine(message);

                    Console.ForegroundColor = defaultColour;
                }
            }

            if (_logger != null) _logger.DoLog(catagory, message);
        }

    }
}
