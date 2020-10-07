using System;
using System.Threading;

namespace ITimer
{
    /// <summary>
    /// Provides a wrapper for the <see cref="System.Threading.Timer" /> implementing the <see cref="ISignaler" /> interface.
    /// </summary>
    public class ThreadingTimer : BaseTimer, ISignaler, IDisposable
    {
        private bool _disposed;
        private readonly Timer _timer;
        private static readonly TimeSpan _noperiod = TimeSpan.FromMilliseconds(-1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadingTimer" /> class, using the specified interval.
        /// </summary>
        /// <param name="interval">The time interval between raising the <see cref="BaseTimer.Elapsed" /> event.</param>
        /// <param name="autoReset">
        ///     Whether the <see cref="ThreadingTimer" /> should raise the <see cref="BaseTimer.Elapsed" /> event only once
        ///     (<c>false</c>) or repeatedly (<c>true</c>).
        /// </param>
        /// <param name="timeProvider">
        ///     When specified, the <c>timeProvider</c> is used to determine the <see cref="TimerElapsedEventArgs.SignalTime" />
        ///     when the <see cref="BaseTimer.Elapsed" /> event is raised. When no <c>timeProvider</c> is specified the
        ///     <see cref="BaseTimer.DefaultTimeProvider" /> is used.
        /// </param>
        public ThreadingTimer(TimeSpan interval, bool autoReset = true, Func<DateTimeOffset> timeProvider = null)
            : base(interval, autoReset, timeProvider)
        {
            _timer = new Timer(TimerCallback, null, Timeout.InfiniteTimeSpan, interval);
        }

        /// <summary>
        /// Starts raising the <see cref="BaseTimer.Elapsed" /> event.
        /// </summary>
        public void Start() => _timer.Change(Interval, AutoReset ? Interval : _noperiod);

        /// <summary>
        /// Stops raising the <see cref="BaseTimer.Elapsed" /> event.
        /// </summary>
        public void Stop() => _timer.Change(Timeout.InfiniteTimeSpan, _noperiod);

        private void TimerCallback(object stateInfo) => OnElapsed(new TimerElapsedEventArgs(TimeProvider.Invoke()));

        #region IDisposable
        /// <summary>
        /// Releases all resources used by the current <see cref="ThreadingTimer" />.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
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

        /// <summary>
        /// Releases all resources used by the current <see cref="ThreadingTimer" />.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}