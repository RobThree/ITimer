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

        [TestMethod]
        public void ThreadingTimer_EnabledPropertyReflectsState()
        {
            var target = new TestTimer();
            Assert.IsFalse(target.Enabled);
            target.Start();
            Assert.IsTrue(target.Enabled);
            target.Start(); // Starting twice shouldn't matter
            Assert.IsTrue(target.Enabled);
            target.Stop();
            Assert.IsFalse(target.Enabled);
            target.Stop();  // Stopping twice shouldn't matter
            Assert.IsFalse(target.Enabled);
        }
    }
}