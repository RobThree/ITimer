using System;
using System.Collections.Generic;
using System.Threading;

namespace ITimer
{
    /// <summary>
    /// Provides a 'timer' to be used in unittests. The <see cref="BaseTimer.Elapsed" /> event will not be raised
    /// automatically by this timer but only when one of the <c>Tick</c> overloads is invoked.
    /// </summary>
    public class TestTimer : BaseTimer, ISignaler
    {
        /// <summary>
        /// Occurs when the <see cref="TestTimer" /> is started.
        /// </summary>
        public event EventHandler<StartedEventArgs> Started;

        /// <summary>
        /// Occurs when the <see cref="TestTimer" /> is stopped.
        /// </summary>
        public event EventHandler<StoppedEventArgs> Stopped;

        /// <summary>
        /// Gets the number of times the timer has ticked since starting the time (or since the last <see cref="Reset" />).
        /// </summary>
        public int TickCount => _tickcount;

        /// <summary>
        /// Gets the number of times the timer was (re)started since starting the time (or since the last <see cref="Reset" />).
        /// </summary>
        public int StartCount => _startcount;

        /// <summary>
        /// Gets the number of times the timer was (re)stopped since starting the time (or since the last <see cref="Reset" />).
        /// </summary>
        public int StopCount => _stopcount;

        private int _tickcount;
        private int _startcount;
        private int _stopcount;
        private readonly bool _requirestart;
        private bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestTimer" /> class with the specified (optional) settings.
        /// </summary>
        /// <param name="timeProvider">The default <see cref="BaseTimer.DefaultTimeProvider" /> to use.</param>
        /// <param name="requireStart">
        /// When <c>true</c>, the timer will not raise the <see cref="BaseTimer.Elapsed" /> event unless the timer
        /// has been started using the <see cref="Start()" /> method. When <c>false</c> the timer doesn't need to be
        /// started.
        /// </param>
        /// <param name="interval">
        /// This value has no effect other than the value being reflected in the <see cref="BaseTimer.Interval" /> propery.
        /// </param>
        /// <param name="autoReset">
        /// This value has no effect other than the value being reflected in the <see cref="BaseTimer.AutoReset" /> propery.
        /// </param>
        /// <remarks>
        /// Since the <see cref="TestTimer" /> is no actual timer, the <paramref name="interval"/> and <paramref name="autoReset"/>
        /// values have no effect other than being reflected in the <see cref="TestTimer" />'s corresponding properties.
        /// </remarks>
        public TestTimer(Func<DateTimeOffset> timeProvider = null, bool requireStart = false, TimeSpan? interval = null, bool autoReset = false)
            : base(interval ?? TimeSpan.Zero, autoReset, timeProvider) => _requirestart = requireStart;

        /// <summary>
        /// Causes the timer to 'tick', i.e. to raise the <see cref="BaseTimer.Elapsed" /> event.
        /// </summary>
        /// <param name="signalTime">
        /// When specified, the <see cref="TimerElapsedEventArgs" /> <see cref="TimerElapsedEventArgs.SignalTime" />
        /// will be set to this value. When <c>null</c>, the <see cref="TestTimer" />'s default time provider is used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the timer was constructed with the <c>requireStart</c> argument set to <c>true</c> and the <see cref="Start()" />
        /// method has not been invoked before invoking this method.
        /// </exception>
        public void Tick(DateTimeOffset? signalTime = null) => Tick(1, (i) => signalTime ?? TimeProvider.Invoke());

        /// <summary>
        /// Causes the timer to 'tick', i.e. to raise the <see cref="BaseTimer.Elapsed" /> event for the specified
        /// amount of times.
        /// </summary>
        /// <param name="ticks">The amount of times the <see cref="BaseTimer.Elapsed" /> event should be raised.</param>
        /// <param name="timeProvider">
        /// The time provider to use when determining the <see cref="TimerElapsedEventArgs.SignalTime" /> for the event.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the number of ticks is less than zero.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the timer was constructed with the <c>requireStart</c> argument set to <c>true</c> and the <see cref="Start()" />
        /// method has not been invoked before invoking this method.
        /// </exception>
        public void Tick(int ticks, Func<int, DateTimeOffset> timeProvider = null)
        {
            if (ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ticks));
            }

            for (var i = 0; i < ticks; i++)
            {
                if (timeProvider != null)
                {
                    RaiseElapsed(timeProvider(i));
                }
                else
                {
                    RaiseElapsed(TimeProvider.Invoke());
                }
            }
        }

        /// <summary>
        /// Causes the timer to 'tick', i.e. to raise the <see cref="BaseTimer.Elapsed" /> event for the number of specified
        /// signaltimes.
        /// </summary>
        /// <param name="signalTimes"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="signalTimes"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the timer was constructed with the <c>requireStart</c> argument set to <c>true</c> and the <see cref="Start()" />
        /// method has not been invoked before invoking this method.
        /// </exception>
        public void Tick(IEnumerable<DateTimeOffset> signalTimes)
        {
            if (signalTimes == null)
            {
                throw new ArgumentNullException(nameof(signalTimes));
            }

            foreach (var s in signalTimes)
            {
                RaiseElapsed(s);
            }
        }

        private void RaiseElapsed(DateTimeOffset signalTime)
        {
            if (_requirestart && !_started)
            {
                throw new InvalidOperationException("Timer must be started");
            }

            OnElapsed(new TestTimerElapsedEventArgs(Interlocked.Increment(ref _tickcount), signalTime));
        }

        /// <summary>
        /// 'Starts' the timer.
        /// </summary>
        /// <remarks>
        /// The <see cref="BaseTimer.Elapsed" /> event won't be automatically raised as normal
        /// timers do. To raise the event, one of the <c>Tick</c> overloads need to be invoked. The <see cref="Started" />
        /// event will be raised to keep track of a <see cref="TestTimer" /> being started. Also, when the <see cref="TestTimer" />
        /// was constructed with the <c>requireStart</c> argument set to <c>true</c>, this method needs to be invoked before
        /// invoking one of the <c>Tick</c> overloads otherwise a <see cref="InvalidOperationException" /> will be thrown.
        /// </remarks>
        public void Start()
        {
            _started = true;
            Interlocked.Increment(ref _startcount);
            Started?.Invoke(this, new StartedEventArgs());
        }

        /// <summary>
        /// 'Starts' the timer.
        /// </summary>
        /// <remarks>
        /// The <see cref="BaseTimer.Elapsed" /> event won't be automatically raised as normal
        /// timers do. To raise the event, one of the <c>Tick</c> overloads need to be invoked. The <see cref="Started" />
        /// event will be raised to keep track of a <see cref="TestTimer" /> being started. Also, when the <see cref="TestTimer" />
        /// was constructed with the <c>requireStart</c> argument set to <c>true</c>, this method needs to be invoked before
        /// invoking one of the <c>Tick</c> overloads otherwise a <see cref="InvalidOperationException" /> will be thrown.
        /// </remarks>
        /// <param name="interval">The time interval between raising the <see cref="BaseTimer.Elapsed" /> event.</param>
        public void Start(TimeSpan interval)
            => Start();

        /// <summary>
        /// 'Stops' the timer.
        /// </summary>
        public void Stop()
        {
            _started = false;
            Interlocked.Increment(ref _stopcount);
            Stopped?.Invoke(this, new StoppedEventArgs());
        }

        /// <summary>
        /// Resets the <see cref="TickCount" />, <see cref="StartCount" /> and <see cref="StopCount" /> properties.
        /// </summary>
        public void Reset()
        {
            _tickcount = 0;
            _startcount = 0;
            _stopcount = 0;
        }

        /// <summary>
        /// Gets a value indicating whether the timer is enabled
        /// </summary>
        public bool Enabled => _started;

        #region IDisposable
        /// <summary>
        /// Releases all resources used by the current <see cref="ThreadingTimer" />.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            //NOP
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

    /// <summary>
    /// Provides data for the <see cref="TestTimer.Started" /> event.
    /// </summary>
    public class StartedEventArgs : EventArgs { };

    /// <summary>
    /// Provides data for the <see cref="TestTimer.Stopped" /> event.
    /// </summary>
    public class StoppedEventArgs : EventArgs { };
}