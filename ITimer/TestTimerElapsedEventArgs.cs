using System;

namespace ITimer
{
    /// <summary>
    /// Provides data for the <see cref="BaseTimer.Elapsed" /> event.
    /// </summary>
    public class TestTimerElapsedEventArgs : TimerElapsedEventArgs
    {
        /// <summary>
        /// Gets the count of ticks since for the <see cref="TestTimer" /> was started (or <see cref="TestTimer.Reset" />).
        /// </summary>
        public int TickCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of a <see cref="TestTimerElapsedEventArgs" />.
        /// </summary>
        /// <param name="tickCount">The count of ticks since for the <see cref="TestTimer" /> was started (or <see cref="TestTimer.Reset" />)</param>
        /// <param name="signalTime">The date/time when the Elapsed event was raised.</param>
        protected internal TestTimerElapsedEventArgs(int tickCount, DateTimeOffset signalTime)
            : base(signalTime)
            => TickCount = tickCount;
    }
}