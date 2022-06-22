using System;

namespace ITimer;

/// <summary>
/// Provides an interface for timers (or, "signalers").
/// </summary>
/// <remarks>
/// Agreed, <see cref="ISignaler" /> is not the best name. <c>ITimer</c> would have been a much better choice, but
/// that conflicts with the namespace. That would require you to write <c>ITimer.ITimer</c> everywhere this interface
/// is used. And since we wanted a simple package-ID and simple (root) namespace we opted for <c>ITimer</c> as namespace
/// and <see cref="ISignaler" /> as interface name. If you have any better suggestions, please let us know and
/// we'll consider it for the next major version.
/// </remarks>
public interface ISignaler : IDisposable
{
    /// <summary>
    /// Occurs when the interval elapses.
    /// </summary>
    event TimerElapsedEventHandler Elapsed;

    /// <summary>
    /// Gets the interval at which the <see cref="Elapsed"/> event is raised.
    /// </summary>
    TimeSpan Interval { get; }

    /// <summary>
    /// Gets a bool indicating whether the timer should raise the <see cref="Elapsed" /> event only
    /// once (<c>false</c>) or repeatedly (<c>true</c>).
    /// </summary>
    bool AutoReset { get; }

    /// <summary>
    /// Starts raising the <see cref="Elapsed" /> event.
    /// </summary>
    void Start();

    /// <summary>
    /// Starts raising the <see cref="Elapsed" /> event with at the specified interval.
    /// </summary>
    /// <param name="interval">The time interval between raising the <see cref="Elapsed" /> event.</param>
    void Start(TimeSpan interval);

    /// <summary>
    /// Stops raising the <see cref="Elapsed" /> event.
    /// </summary>
    void Stop();

    /// <summary>
    /// Gets a value indicating whether the timer is enabled.
    /// </summary>
    public bool Enabled { get; }
}
