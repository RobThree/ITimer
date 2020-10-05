using System;

namespace ITimer
{
    public class TimerElapsedEventArgs : EventArgs
    {
        public DateTimeOffset SignalTime { get; private set; }

        public TimerElapsedEventArgs(DateTimeOffset signalTime) => SignalTime = signalTime;
    }
}