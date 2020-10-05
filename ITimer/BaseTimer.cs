using System;

namespace ITimer
{
    public delegate void ElapsedEventHandler(object sender, TimerElapsedEventArgs e);

    public abstract class BaseTimer
    {
        public event ElapsedEventHandler Elapsed;

        protected static readonly Func<DateTimeOffset> _defaulttimeprovider = () => DateTimeOffset.Now;

        public TimeSpan Interval { get; private set; }
        public bool AutoReset { get; private set; }
        public Func<DateTimeOffset> TimeProvider { get; private set; }

        public BaseTimer(TimeSpan interval, bool autoReset, Func<DateTimeOffset> timeProvider)
        {
            double roundedInterval = Math.Ceiling(interval.TotalMilliseconds);
            if (roundedInterval > int.MaxValue || roundedInterval < 0)
                throw new ArgumentOutOfRangeException(nameof(interval));
            
            Interval = interval;
            AutoReset = autoReset;
            TimeProvider = timeProvider ?? _defaulttimeprovider;
        }

        protected virtual void OnElapsed(TimerElapsedEventArgs e)
        {
            Elapsed?.Invoke(this, e);
        }
    }
}