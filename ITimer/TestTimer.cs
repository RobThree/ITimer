using System;
using System.Collections.Generic;
using System.Threading;

namespace ITimer
{
    public class TestTimer : BaseTimer, ITimer
    {
        public event EventHandler<StartedEventArgs> Started;
        public event EventHandler<StoppedEventArgs> Stopped;

        public int TickCount => _tickcount;
        public int StartCount => _startcount;
        public int StopCount => _endcount;

        private int _tickcount;
        private int _startcount;
        private int _endcount;

        public TestTimer(Func<DateTimeOffset> timeProvider = null)
            : base(TimeSpan.Zero, false, timeProvider) { }

        public void Tick(DateTimeOffset? signalTime = null) => Tick(1, (i) => signalTime ?? TimeProvider.Invoke());
        public void Tick(int ticks, Func<int, DateTimeOffset> timeProvider = null)
        {
            if (ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(ticks));

            for (var i = 0; i < ticks; i++)
            {
                if (timeProvider != null)
                    RaiseElapsed(timeProvider(i));
                else
                    RaiseElapsed(TimeProvider.Invoke());
            }
        }

        public void Tick(IEnumerable<DateTimeOffset> signalTimes)
        {
            if (signalTimes == null)
                throw new ArgumentNullException(nameof(signalTimes));

            foreach (var s in signalTimes)
                RaiseElapsed(s);
        }

        private void RaiseElapsed(DateTimeOffset signalTime)
        {
            OnElapsed(new TestTimerElapsedEventArgs(Interlocked.Increment(ref _tickcount), signalTime));
        }

        public void Start()
        {
            Interlocked.Increment(ref _startcount);
            Started?.Invoke(this, new StartedEventArgs());
        }
        public void Stop()
        {
            Interlocked.Increment(ref _endcount);
            Stopped?.Invoke(this, new StoppedEventArgs());
        }

        public void Reset()
        {
            _tickcount = 0;
            _startcount = 0;
            _endcount = 0;
        }
    }

    public class StartedEventArgs : EventArgs { };
    public class StoppedEventArgs : EventArgs { };
}