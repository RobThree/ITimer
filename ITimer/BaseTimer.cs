using System;

namespace ITimer;

/// <summary>
/// Represents the method that will handle the <see cref="BaseTimer.Elapsed" /> event of an <see cref="ISignaler" />.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">An <see cref="TimerElapsedEventArgs" /> object that contains the event data.</param>
public delegate void TimerElapsedEventHandler(object sender, TimerElapsedEventArgs e);

/// <summary>
/// Provides a baseclass for timers.
/// </summary>
public abstract class BaseTimer : ISignaler
{
    /// <inheritdoc/>
    public event TimerElapsedEventHandler? Elapsed;

    /// <summary>
    /// Defines the default interval for timers
    /// </summary>
    public static readonly TimeSpan DEFAULTINTERVAL = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Returns the default time provider; a function that returns the time to determine the signaltime
    /// for the <see cref="TimerElapsedEventArgs" /> used in the <see cref="Elapsed" /> event.
    /// </summary>
    /// <remarks>Uses <see cref="DateTimeOffset.Now" /> but it's implementation may change in the future.</remarks>
    public static Func<DateTimeOffset> DefaultTimeProvider { get; } = () => DateTimeOffset.Now;

    /// <inheritdoc/>
    public TimeSpan Interval { get; protected set; }

    /// <inheritdoc/>
    public bool AutoReset { get; private set; }

    /// <summary>
    /// Returns the timer's time provider; a function that returns the time to determine the signaltime
    /// for the <see cref="TimerElapsedEventArgs" /> used in the <see cref="Elapsed" /> event.
    /// </summary>
    public Func<DateTimeOffset> TimeProvider { get; private set; }

    /// <inheritdoc/>
    public abstract bool Enabled { get; }

    /// <summary>
    /// Initializes a new instance of a <see cref="BaseTimer" /> with the given interval, autoreset setting and
    /// time provider.
    /// </summary>
    /// <param name="interval">The interval at which to raise the <see cref="Elapsed" /> event.</param>
    /// <param name="autoReset">
    ///     Specifies whether the timer should raise the <see cref="Elapsed" /> event only once (<c>false</c>) or
    ///     repeatedly (<c>true</c>).
    /// </param>
    /// <param name="timeProvider">
    ///     The function that returns the time to determine the signaltime for the <see cref="TimerElapsedEventArgs" />
    ///     used in the <see cref="Elapsed" /> event. Defaults to the <see cref="DefaultTimeProvider" /> when unspecified
    ///     (<c>null</c>).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the specified <paramref name="interval"/> is less than <see cref="TimeSpan.Zero" /> or 
    ///     exceeds the maximum time of <see cref="int.MaxValue" /> milliseconds.
    /// </exception>
    public BaseTimer(TimeSpan interval, bool autoReset, Func<DateTimeOffset>? timeProvider)
    {
        if (interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval));
        }

        var roundedInterval = Math.Ceiling(interval.TotalMilliseconds);
        if (roundedInterval is > int.MaxValue or < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(interval));
        }

        Interval = interval;
        AutoReset = autoReset;
        TimeProvider = timeProvider ?? DefaultTimeProvider;
    }

    /// <summary>
    /// Raises the <see cref="Elapsed" /> event in a safe way.
    /// </summary>
    /// <param name="e">
    /// The <see cref="TimerElapsedEventArgs" /> to raise the <see cref="Elapsed" /> event with.
    /// </param>
    protected virtual void OnElapsed(TimerElapsedEventArgs e) => Elapsed?.Invoke(this, e);

    /// <inheritdoc/>
    public abstract void Start();

    /// <inheritdoc/>
    public abstract void Start(TimeSpan interval);

    /// <inheritdoc/>
    public abstract void Stop();

    /// <inheritdoc/>
    public abstract void Dispose();
}