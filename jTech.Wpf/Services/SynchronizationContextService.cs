using jTech.Common.Core;
using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace jTech.Wpf.Services
{
    public interface ISynchronizationContextService
    {
        void SetApplicationUiScheduler();
        IScheduler GetNewBackgroundWorker(string workerName);
        TaskScheduler ApplicationUiTaskScheduler { get; }
        DispatcherScheduler ApplicationUiDispatcherScheduler { get; }
    }

    public class SynchronizationContextService : ISynchronizationContextService
    {
        private readonly OneTimeAction _setUiSchedulers;

        private TaskScheduler _uiTaskScheduler;
        private DispatcherScheduler _uiDispatcherScheduler;

        public SynchronizationContextService()
        {
            _setUiSchedulers = new OneTimeAction(DoSetApplicationUiScheduler);
        }

        public void SetApplicationUiScheduler()
        {
            _setUiSchedulers.Execute();
        }

        public IScheduler GetNewBackgroundWorker(string workerName)
        {
            var worker = new EventLoopScheduler(ts => new Thread(ts)
            {
                IsBackground = true,
                Name = workerName,
            });

            return worker;
        }

        public DispatcherScheduler ApplicationUiDispatcherScheduler => GetApplicationUiDispatcherScheduler();

        public TaskScheduler ApplicationUiTaskScheduler => GetApplicationUiTaskScheduler();

        private void DoSetApplicationUiScheduler()
        {
            var dispatcher = Dispatcher.FromThread(Thread.CurrentThread); //7:30
            if (dispatcher == null)
            {
                throw new InvalidOperationException("Current thread of execution does not have an associated dispatcher. This method must be called from the application IU thread.");
            }

            _uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _uiDispatcherScheduler = DispatcherScheduler.Current;

            if (_uiTaskScheduler == null ||
                _uiDispatcherScheduler == null)
            {
                throw new InvalidOperationException($"Application UI TaskScheduler and DispatcherScheduler could not been set. Call {nameof(SetApplicationUiScheduler)} from the UI thread to capture this context.");
            }
        }

        private TaskScheduler GetApplicationUiTaskScheduler()
        {
            var scheduler = _uiTaskScheduler;
            if (scheduler == null)
            {
                throw new InvalidOperationException($"Application UI TaskScheduler has not been set. Call {nameof(SetApplicationUiScheduler)} from the UI thread to capture this context.");
            }

            return scheduler;
        }

        private DispatcherScheduler GetApplicationUiDispatcherScheduler()
        {
            var scheduler = _uiDispatcherScheduler;
            if (scheduler == null)
            {
                throw new InvalidOperationException($"Application UI DispatcherScheduler has not been set. Call {nameof(SetApplicationUiScheduler)} from the UI thread to capture this context.");
            }

            return scheduler;
        }

    }
}
