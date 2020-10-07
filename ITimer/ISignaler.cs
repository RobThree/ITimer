using System;

namespace ITimer
{
    /// <summary>
    /// Provides an interface for timers (or, "signalers").
    /// </summary>
    /// <remarks>
    /// Yes, <see cref="ISignaler" /> is not the best name. <c>ITimer</c> would have been a much better choice, but
    /// that conflicts with the namespace. That would require you to write ITimer.ITimer everywhere this interface
    /// is used. And since we wanted a simple package-ID and simple (root) namespace we opted for ITimer as namespace
    /// and <see cref="ISignaler" /> as interface name. If you have any better suggestions, please let us know and
    /// we'll consider it for the next major version.
    /// </remarks>
    public interface ISignaler
    {
        /// <summary>
        /// Occurs when the interval elapses.
        /// </summary>
        event TimerElapsedEventHandler Elapsed;

        /// <summary>
        /// Gets the interval at which the Elapsed event is raised.
        /// </summary>
        TimeSpan Interval { get; }

        /// <summary>
        /// Gets or sets whether the Timer should raise the Elapsed event only once (false) or repeatedly (true).
        /// </summary>
        bool AutoReset { get; }

        /// <summary>
        /// Starts raising the <see cref="Elapsed" /> event.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops raising the <see cref="Elapsed" /> event
        /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
        void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
    }
}
