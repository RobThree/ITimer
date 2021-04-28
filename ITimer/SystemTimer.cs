using System;
using System.ComponentModel;
using System.Timers;

namespace ITimer
{
    /// <summary>
    /// Provides a wrapper for the <see cref="System.Timers.Timer" /> implementing the <see cref="ISignaler" /> interface.
    /// </summary>
    public class SystemTimer : BaseTimer, ISignaler
    {
        private bool _disposed;
        private readonly Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemTimer" /> class, using the <see cref="BaseTimer.DEFAULTINTERVAL"/>.
        /// </summary>
        /// <param name="autoReset">
        ///     Whether the <see cref="SystemTimer" /> should raise the <see cref="BaseTimer.Elapsed" /> event only once
        ///     (<c>false</c>) or repeatedly (<c>true</c>).
        /// </param>
        /// <param name="timeProvider">
        ///     When specified, the <c>timeProvider</c> is used to determine the <see cref="TimerElapsedEventArgs.SignalTime" />
        ///     when the <see cref="BaseTimer.Elapsed" /> event is raised. When no <c>timeProvider</c> is specified the
        ///     <see cref="BaseTimer.DefaultTimeProvider" /> is used.
        /// </param>
        /// <param name="synchronizingObject">The object used to marshal event-handler calls that are issued when an interval has elapsed.</param>
        public SystemTimer(bool autoReset = true, Func<DateTimeOffset> timeProvider = null, ISynchronizeInvoke synchronizingObject = null)
            : this(DEFAULTINTERVAL, autoReset, timeProvider, synchronizingObject) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemTimer" /> class, using the specified interval.
        /// </summary>
        /// <param name="interval">The time interval between raising the <see cref="BaseTimer.Elapsed" /> event.</param>
        /// <param name="autoReset">
        ///     Whether the <see cref="SystemTimer" /> should raise the <see cref="BaseTimer.Elapsed" /> event only once
        ///     (<c>false</c>) or repeatedly (<c>true</c>).
        /// </param>
        /// <param name="timeProvider">
        ///     When specified, the <c>timeProvider</c> is used to determine the <see cref="TimerElapsedEventArgs.SignalTime" />
        ///     when the <see cref="BaseTimer.Elapsed" /> event is raised. When no <c>timeProvider</c> is specified the
        ///     <see cref="BaseTimer.DefaultTimeProvider" /> is used.
        /// </param>
        /// <param name="synchronizingObject">The object used to marshal event-handler calls that are issued when an interval has elapsed.</param>
        public SystemTimer(TimeSpan interval, bool autoReset = true, Func<DateTimeOffset> timeProvider = null, ISynchronizeInvoke synchronizingObject = null)
            : base(interval, autoReset, timeProvider)
        {
            _timer = new Timer(Interval.TotalMilliseconds) { AutoReset = AutoReset };
            _timer.SynchronizingObject = synchronizingObject;
            _timer.Elapsed += (s, e) => OnElapsed(new TimerElapsedEventArgs(TimeProvider?.Invoke() ?? e.SignalTime));
        }

        /// <summary>
        /// Starts raising the <see cref="BaseTimer.Elapsed" /> event.
        /// </summary>
        public void Start() => _timer.Start();

        /// <summary>
        /// Starts raising the <see cref="BaseTimer.Elapsed" /> event with at the specified <paramref name="interval"/>.
        /// </summary>
        /// <param name="interval">The time interval between raising the <see cref="BaseTimer.Elapsed" /> event.</param>
        public void Start(TimeSpan interval)
        {
            Interval = interval;
            _timer.Interval = interval.TotalMilliseconds;
            _timer.Start();
        }

        /// <summary>
        /// Stops raising the <see cref="BaseTimer.Elapsed" /> event.
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that are issued when an interval has elapsed.
        /// </summary>
        public ISynchronizeInvoke SynchronizingObject
        {
            get => _timer.SynchronizingObject;
            set => _timer.SynchronizingObject = value;
        }

        #region IDisposable
        /// <summary>
        /// Releases all resources used by the current <see cref="SystemTimer" />.
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
                    _timer?.Stop();
                    _timer?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the current <see cref="SystemTimer" />.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion;
    }
}