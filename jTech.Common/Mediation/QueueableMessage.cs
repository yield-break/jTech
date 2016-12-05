using System.Threading;

namespace jTech.Common.Mediation
{
    public class QueueableMessageBase
    {
        private static long _queueCounter;

        protected static long NewQueueId()
        {
            return Interlocked.Increment(ref _queueCounter);
        }

    }
    public class QueueableMessage<T> : QueueableMessageBase, IQueueable<T>
    {
        public QueueableMessage(T message)
        {
            QueueId = NewQueueId();
            Message = message;
        }

        public long QueueId { get; protected set; }
        public T Message { get; protected set; }

    }
}
