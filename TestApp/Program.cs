using ITimer;
using System;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var i = TimeSpan.FromSeconds(1);

            var timers = new ITimer.ITimer[] {
                new ThreadingTimer(i, true),
                new SystemTimer(i, true),
                new TestTimer()
            };

            foreach (var t in timers)
            {
                t.Elapsed += (s, e) => Console.WriteLine($"{s.GetType().Name,-20}: {e.SignalTime:O}");
                t.Start();
            }

            timers[0].Elapsed += (s, e) => ((TestTimer)timers[2]).Tick();

            Console.WriteLine($"Start: {DateTime.Now}");
            Console.ReadKey();

            foreach (var t in timers)
                t.Stop();

            Console.WriteLine($"Stop: {DateTime.Now}");
            Console.ReadKey();
        }
    }
}
