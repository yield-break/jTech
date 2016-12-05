using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jTech.Common.Threading
{
    public class LongRunningTask
    {
        private readonly object _stateLock = new object();
        private readonly Action _action;

        public LongRunningTask(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            _action = action;
        }

        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public Task Task { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start()
        {
            // Double check locking.
            if (IsRunning) return;
            lock (_stateLock)
            {
                if (IsRunning) return;

                if (CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested)
                {
                    CancellationTokenSource.Cancel();
                }

                CancellationTokenSource = new CancellationTokenSource();
                Task = Task.Factory.StartNew(_action, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                IsRunning = true;
            }
        }

        public void Cancel()
        {
            if (CancellationTokenSource == null || CancellationTokenSource.IsCancellationRequested) return;
            CancellationTokenSource.Cancel();
        }

        public void Stop()
        {
            // Double check locking.
            if (!IsRunning) return;
            lock (_stateLock)
            {
                if (!IsRunning) return;

                Cancel();

                if (Task != null && CancellationTokenSource != null)
                {
                    try
                    {
                        // Wait for a maximum of 10 seconds to let the task finish what it was doing before killing it off.
                        Task.Wait(TimeSpan.FromSeconds(10));
                    }
                    catch (AggregateException aggregateException)
                    {
                        // If task cancellation has gone through already, an OperationCanceledException will be thrown. Handle these.
                        aggregateException.Handle(e => e is OperationCanceledException);
                    }
                }

                IsRunning = false;
            }
        }

    }
}
