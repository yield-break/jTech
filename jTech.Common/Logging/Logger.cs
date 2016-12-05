using System;
using System.Linq;
using jTech.Common.Reactive;
using jTech.Common.Reactive.Events;

namespace jTech.Common.Logging
{
    public abstract class Logger : ILogger
    {
        protected readonly Type Caller;

        protected Logger(Type caller, IEventPublisher<IServiceRequest> eventPublisher) : this(caller)
        {
            eventPublisher.GetEvent<DayRollRequest>()
                          .Subscribe(PerformDayRoll);
        }
        protected Logger(Type caller)
        {
            Caller = caller;
        }

        public virtual void Log(LogCatagory catagory, string message)
        {
            Log(catagory, message, new object[0]);
        }

        public virtual void Log(LogCatagory catagory, string message, params object[] args)
        {
            DoLog(catagory, CreateLog(catagory, message, args));
        }

        public virtual void Log(LogCatagory catagory, Exception e, string message)
        {
            Log(catagory, e, message, new object[0]);
        }

        public virtual void Log(LogCatagory catagory, Exception e, string message, params object[] args)
        {
            DoLog(catagory, CreateErrorLog(CreateLog(catagory, message, args), e));
        }

        protected virtual string CreateLog(LogCatagory catagory, string message, params object[] args)
        {
            return String.Format("{0} {1}: {2} - {3}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), Caller, catagory, args.Any() ? String.Format(message, args) : message);
        }

        protected virtual string CreateErrorLog(string message, Exception e)
        {
            return String.Format("{0}{1}{2}",  message, Environment.NewLine, e);
        }

        protected internal abstract void DoLog(LogCatagory catagory, string message);

        private void PerformDayRoll(DayRollRequest request)
        {
            Log(LogCatagory.Info, "End of day request from '{0}'. Reason: '{1}'", request.Sender, request.Reason);
            OnDayRoll();
        }

        protected internal virtual void OnDayRoll()
        {

        }

    }
}
