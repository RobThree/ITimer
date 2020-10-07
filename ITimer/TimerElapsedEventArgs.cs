using System;

namespace ITimer
{
    /// <summary>
    /// Provides data for the <see cref="BaseTimer.Elapsed" /> event.
    /// </summary>
    public class TimerElapsedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the date/time when the Elapsed event was raised.
        /// </summary>
        public DateTimeOffset SignalTime { get; private set; }

        /// <summary>
        /// Initializes a new instance of a <see cref="TimerElapsedEventArgs" />.
        /// </summary>
        /// <param name="signalTime">The date/time when the Elapsed event was raised.</param>
        protected internal TimerElapsedEventArgs(DateTimeOffset signalTime)
        {
            SignalTime = signalTime;
        }
    }
}