using System;
using System.Timers;

namespace ITimer
{
    public class SystemTimer : BaseTimer, ISignaler, IDisposable
    {
        private bool _disposed;
        private readonly Timer _timer;

        public SystemTimer(TimeSpan interval, bool autoReset = true, Func<DateTimeOffset> timeProvider = null)
            :base(interval, autoReset, timeProvider)
        {
            _timer = new Timer(Interval.TotalMilliseconds) { AutoReset = AutoReset };
            _timer.Elapsed += (s, e) => OnElapsed(new TimerElapsedEventArgs(TimeProvider?.Invoke() ?? e.SignalTime));
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer?.Stop();
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
        #endregion;
    }
}