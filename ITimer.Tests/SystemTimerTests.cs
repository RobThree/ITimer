using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ITimer.Tests
{
    [TestClass]
    public class SystemTimerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SystemTimer_Throws_OnNegativeInterval()
        {
            var _ = new SystemTimer(TimeSpan.FromTicks(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SystemTimer_Throws_OnTooBigInterval()
        {
            var _ = new SystemTimer(TimeSpan.FromTicks(long.MaxValue));
        }
    }
}