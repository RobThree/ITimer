using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ITimer.Tests;

[TestClass]
public class PeriodicTimerTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PeriodicTimer_Throws_OnNegativeInterval()
    {
        var _ = new PeriodicTimer(TimeSpan.FromTicks(-1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PeriodicTimer_Throws_OnTooBigInterval()
    {
        var _ = new PeriodicTimer(TimeSpan.FromTicks(long.MaxValue));
    }

    [TestMethod]
    public void PeriodicTimer_EnabledPropertyReflectsState()
    {
        var target = new PeriodicTimer();
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