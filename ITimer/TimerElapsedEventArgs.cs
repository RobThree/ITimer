using System;

namespace ITimer
{
    public class TimerElapsedEventArgs : EventArgs
    {
        public DateTimeOffset SignalTime { get; private set; }

        public TimerElapsedEventArgs(DateTimeOffset signalTime)
        {
            SignalTime = signalTime;
        }
    }

    public class TestTimerElapsedEventArgs : TimerElapsedEventArgs
    {
        public int TickCount { get; private set; }

        public TestTimerElapsedEventArgs(int tickCount, DateTimeOffset signalTime)
            :base(signalTime)
        {
            TickCount = tickCount;
        }
    }

}