using System;
using System.Threading;

namespace ITimer;

#if NET6_0_OR_GREATER
/// <summary>
/// Provides a wrapper for the <see cref="System.Threading.PeriodicTimer" /> implementing the <see cref="ISignaler" /> interface.
/// </summary>
public class PeriodicTimer : BaseTimer, ISignaler
{
    private bool _disposed;
    private CancellationTokenSource? _cts;

    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicTimer" /> class, using the <see cref="BaseTimer.DEFAULTINTERVAL"/>.
    /// </summary>
    /// <param name="timeProvider">
    ///     When specified, the <c>timeProvider</c> is used to determine the <see cref="TimerElapsedEventArgs.SignalTime" />
    ///     when the <see cref="BaseTimer.Elapsed" /> event is raised. When no <c>timeProvider</c> is specified the
    ///     <see cref="BaseTimer.DefaultTimeProvider" /> is used.
    /// </param>
    public PeriodicTimer(Func<DateTimeOffset>? timeProvider = null)
        : this(DEFAULTINTERVAL, timeProvider) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicTimer" /> class, using the specified interval.
    /// </summary>
    /// <param name="interval">The time interval between raising the <see cref="BaseTimer.Elapsed" /> event.</param>
    /// <param name="timeProvider">
    ///     When specified, the <c>timeProvider</c> is used to determine the <see cref="TimerElapsedEventArgs.SignalTime" />
    ///     when the <see cref="BaseTimer.Elapsed" /> event is raised. When no <c>timeProvider</c> is specified the
    ///     <see cref="BaseTimer.DefaultTimeProvider" /> is used.
    /// </param>
    public PeriodicTimer(TimeSpan interval, Func<DateTimeOffset>? timeProvider = null)
        : base(interval, true, timeProvider) { }

    /// <inheritdoc/>
    public override bool Enabled => !_cts?.Token.IsCancellationRequested ?? false;

    /// <inheritdoc/>
    public override void Start()
        => Start(Interval);

    /// <inheritdoc/>
    public override void Start(TimeSpan interval)
        => Start(interval, default);

    /// <summary>
    /// Starts raising the <see cref="BaseTimer.Elapsed" /> event with at the specified interval.
    /// </summary>
    /// <param name="interval">The time interval between raising the <see cref="BaseTimer.Elapsed" /> event.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    public async void Start(TimeSpan interval, CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        using var timer = new System.Threading.PeriodicTimer(interval);
        try
        {
            while (await timer.WaitForNextTickAsync(_cts.Token).ConfigureAwait(false))
            {
                OnElapsed(new TimerElapsedEventArgs(TimeProvider.Invoke()));
            }
        }
        catch (OperationCanceledException) { }
    }

    /// <inheritdoc/>
    public override void Stop() => _cts?.Cancel();

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
                _cts?.Dispose();
            }
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
#endif