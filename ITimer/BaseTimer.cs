using System;

namespace ITimer
{
    public delegate void ElapsedEventHandler(object sender, TimerElapsedEventArgs e);

    public abstract class BaseTimer
    {
        public event ElapsedEventHandler Elapsed;

        protected static readonly Func<DateTimeOffset> DEFAULTTIMEPROVIDER = () => DateTimeOffset.Now;

        public static Func<DateTimeOffset> DefaultTimeProvider { get => DEFAULTTIMEPROVIDER; }

        public TimeSpan Interval { get; private set; }
        public bool AutoReset { get; private set; }
        public Func<DateTimeOffset> TimeProvider { get; private set; }

        public BaseTimer(TimeSpan interval, bool autoReset, Func<DateTimeOffset> timeProvider)
        {
            if (interval < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(interval));

            double roundedInterval = Math.Ceiling(interval.TotalMilliseconds);
            if (roundedInterval > int.MaxValue || roundedInterval < 0)
                throw new ArgumentOutOfRangeException(nameof(interval));

            Interval = interval;
            AutoReset = autoReset;
            TimeProvider = timeProvider ?? DEFAULTTIMEPROVIDER;
        }

        protected virtual void OnElapsed(TimerElapsedEventArgs e)
        {
            Elapsed?.Invoke(this, e);
        }
    }
}