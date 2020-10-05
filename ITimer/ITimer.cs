using System;

namespace ITimer
{
    public interface ITimer
    {
        event ElapsedEventHandler Elapsed;
        TimeSpan Interval { get; }
        bool AutoReset { get; }
        void Start();
#pragma warning disable CA1716 // Identifiers should not match keywords
        void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
    }
}
