using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jTech.Common.Logging;
using jTech.Common.Threading;
using System.Collections.Concurrent;

namespace jTech.Common.Mediation
{
    public interface IQueuingModule<T> : IDisposable
    {
        string ModuleName { get; }
        bool IsInitialised { get; }
        bool IsRunning { get; }
        BlockingCollection<IQueueable<T>> IncomingQueue { get; }
        BlockingCollection<IQueueable<T>> OutgoingQueue { get; }

        void Start();
        void Stop();
    }

    public class QueuingModule<T> : IQueuingModule<T>
    {
        private readonly object _initialiseLock = new object();
        private readonly object _stateLock = new object();

        protected ILogger Log;
        protected LongRunningTask LongRunningTask { get; set; }

        public QueuingModule(ILogger logger, string moduleName) : this (moduleName)
        {
            Log = logger;
        }
        public QueuingModule(string moduleName)
        {
            ModuleName = moduleName;
        }

        public string ModuleName { get; protected set; }
        public bool IsInitialised { get; protected set; }
        public bool IsRunning { get; protected set; }
        public BlockingCollection<IQueueable<T>> IncomingQueue { get; protected set; }
        public BlockingCollection<IQueueable<T>> OutgoingQueue { get; protected set; }

        public virtual void Initialise()
        {
            if (IsInitialised) return;
            lock(_initialiseLock)
            {
                if (IsInitialised) return;

                Log.Log(LogCatagory.Info, "Initialising {0}. . .", ModuleName);

                OnInitialise();

                Log.Log(LogCatagory.Info, "{0} Initialised.", ModuleName);
                IsInitialised = true;
            }
        }

        protected virtual void OnInitialise() { }

        public virtual void Start()
        {
            if (IsRunning) return;
            lock (_stateLock)
            {
                if (IsRunning) return;
                Log.Log(LogCatagory.Info, "Starting {0}. . .", ModuleName);

                LongRunningTask = new LongRunningTask(AttendIncomingQueue);
                LongRunningTask.Start();

                OnStart();

                Log.Log(LogCatagory.Info, "{0} started.", ModuleName);
                IsRunning = true;
            }
        }
        
        protected virtual void OnStart() { }

        public virtual void Stop()
        {
            if (!IsRunning) return;
            lock (_stateLock)
            {
                if (!IsRunning) return;
                Log.Log(LogCatagory.Info, "Stoping {0}. . .", ModuleName);

                OnStop();

                if (LongRunningTask != null) LongRunningTask.Stop();

                Log.Log(LogCatagory.Info, "{0} stopped.", ModuleName);
                IsRunning = false;
            }
        }

        protected virtual void OnStop() { }

        protected virtual void AttendIncomingQueue()
        {
            try
            {
                foreach(IQueueable<T> message in IncomingQueue.GetConsumingEnumerable(LongRunningTask.CancellationTokenSource.Token))
                {
                    Log.Log(LogCatagory.Info, "{0} recieved message with QueueId: {1}", ModuleName, message.QueueId);
                    OnIncomingMessage(message);
                    Log.Log(LogCatagory.Info, "{0} finished processing message with QueueId: {1}.", ModuleName, message.QueueId);
                }
            }
            catch(OperationCanceledException)
            {
                Log.Log(LogCatagory.Info, "{0} shutting down.", ModuleName);
            }
            catch (Exception e)
            {
                Log.Log(LogCatagory.Error, e, "{0}: Unexpected exception caught while attending incoming queue.", ModuleName);
            }
        }

        protected virtual void OnIncomingMessage(IQueueable<T> message) { }

        public void Dispose()
        {
            try
            {
                Stop();
            }
            catch (Exception)
            {
                // Gulp!
            }
            
            try
            {
                OnDispose();
                IsInitialised = false;
            }
            catch (Exception)
            {
                // Gulp!
            }
        }

        protected virtual void OnDispose()
        {

        }

    }
}
