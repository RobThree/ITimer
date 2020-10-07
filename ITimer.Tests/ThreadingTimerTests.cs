using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ITimer.Tests
{
    [TestClass]
    public class ThreadingTimerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThreadingTimer_Throws_OnNegativeInterval()
        {
            var _ = new ThreadingTimer(TimeSpan.FromTicks(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThreadingTimer_Throws_OnTooBigInterval()
        {
            var _ = new ThreadingTimer(TimeSpan.FromTicks(long.MaxValue));
        }
    }
}