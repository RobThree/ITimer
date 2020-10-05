using System;
using System.Threading;

namespace ITimer
{
    public class ThreadingTimer : BaseTimer, ISignaler, IDisposable
    {
        private bool _disposed;
        private readonly Timer _timer;
        private static readonly TimeSpan _noperiod = TimeSpan.FromMilliseconds(-1);

        public ThreadingTimer(TimeSpan interval, bool autoReset = true, Func<DateTimeOffset> timeProvider = null)
            :base(interval, autoReset, timeProvider)
        {
            _timer = new Timer(TimerCallback, null, Timeout.InfiniteTimeSpan, interval);
        }

        public void TimerCallback(object stateInfo) => OnElapsed(new TimerElapsedEventArgs(TimeProvider.Invoke()));
        public void Start() => _timer.Change(Interval, AutoReset ? Interval : _noperiod);
        public void Stop() => _timer.Change(Timeout.InfiniteTimeSpan, _noperiod);

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    _timer?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}