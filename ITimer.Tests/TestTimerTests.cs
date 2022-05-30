using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ITimer.Tests
{
    [TestClass]
    public class TestTimerTests
    {
        private static readonly DateTimeOffset TESTVALUE1 = new(2013, 12, 11, 10, 9, 8, 7, TimeSpan.FromHours(6));
        private static readonly DateTimeOffset TESTVALUE2 = TESTVALUE1.AddDays(1);
        private static readonly DateTimeOffset TESTVALUE3 = TESTVALUE2.AddMonths(1);
        private static readonly DateTimeOffset[] TESTVALUES = { TESTVALUE1, TESTVALUE2, TESTVALUE3 };

        [TestMethod]
        public void TestTimer_UsesCorrect_Defaults()
        {
            var target = new TestTimer();

            Assert.AreEqual(target.Interval, TimeSpan.Zero);
            Assert.IsFalse(target.AutoReset);
            ReferenceEquals(target.TimeProvider, BaseTimer.DefaultTimeProvider);
        }

        [TestMethod]
        public void TestTimer_Tick_ProvidesCorrect_SignalTime()
        {
            var result = DateTimeOffset.MinValue;
            var target = new TestTimer(() => TESTVALUE1);
            target.Elapsed += (s, e) => { result = e.SignalTime; };

            target.Tick();                          // Use TestTimer timeprovider from constructor
            Assert.AreEqual(TESTVALUE1, result);

            target.Tick(TESTVALUE2);                // Use provided value
            Assert.AreEqual(TESTVALUE2, result);

            target.Tick(1, (i) => TESTVALUE3);      // Use provided timeprovider
            Assert.AreEqual(TESTVALUE3, result);
        }

        [TestMethod]
        public void TestTimer_UsesNow_AsDefaultSignalTime()
        {
            var result = DateTimeOffset.MinValue;
            var target = new TestTimer();
            target.Elapsed += (s, e) => { result = e.SignalTime; };

            target.Tick();
            Assert.IsTrue(DateTimeOffset.Now - result < TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void TestTimer_Counters_AreCorrect()
        {
            var eventcount = 0;
            var target = new TestTimer();
            target.Elapsed += (s, e) => { eventcount++; };

            target.Start();
            target.Tick(3);
            target.Stop();
            target.Start();

            Assert.AreEqual(2, target.StartCount);
            Assert.AreEqual(1, target.StopCount);
            Assert.AreEqual(3, target.TickCount);

            target.Reset();

            Assert.AreEqual(0, target.StartCount);
            Assert.AreEqual(0, target.StopCount);
            Assert.AreEqual(0, target.TickCount);
        }

        [TestMethod]
        public void TestTimer_StartStopTickEventsAreInvoked()
        {
            var started = false;
            var ticked = false;
            var stopped = false;

            var target = new TestTimer();
            target.Started += (s, e) => { started = true; };
            target.Stopped += (s, e) => { stopped = true; };
            target.Elapsed += (s, e) => { ticked = true; };

            Assert.IsFalse(started);
            Assert.IsFalse(stopped);
            Assert.IsFalse(ticked);

            target.Start();

            Assert.IsTrue(started);
            Assert.IsFalse(ticked);
            Assert.IsFalse(stopped);

            target.Tick();

            Assert.IsTrue(started);
            Assert.IsTrue(ticked);
            Assert.IsFalse(stopped);

            target.Stop();

            Assert.IsTrue(started);
            Assert.IsTrue(ticked);
            Assert.IsTrue(stopped);
        }

        [TestMethod]
        public void TestTimer_MultiTick_TimeProvider_ReturnsCorrectTickCountValuesAndSignalTimes()
        {
            var results = new List<KeyValuePair<int, DateTimeOffset>>();

            var target = new TestTimer();

            target.Elapsed += (s, e) => { results.Add(new KeyValuePair<int, DateTimeOffset>(((TestTimerElapsedEventArgs)e).TickCount, e.SignalTime)); };
            target.Tick(3, (i) => TESTVALUES[i]);

            Assert.AreEqual(3, results.Count);
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(results[i].Key, i + 1);
                Assert.AreEqual(results[i].Value, TESTVALUES[i]);
            }
        }

        [TestMethod]
        public void TestTimer_MultiTick_Enumerable_ReturnsCorrectTickCountValuesAndSignalTimes()
        {
            var results = new List<KeyValuePair<int, DateTimeOffset>>();

            var target = new TestTimer();

            target.Elapsed += (s, e) => { results.Add(new KeyValuePair<int, DateTimeOffset>(((TestTimerElapsedEventArgs)e).TickCount, e.SignalTime)); };
            target.Tick(TESTVALUES);

            Assert.AreEqual(3, results.Count);
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(results[i].Key, i + 1);
                Assert.AreEqual(results[i].Value, TESTVALUES[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestTimer_MultiTick_TimeProvider_ThrowsOnNegativeTicks()
        {
            var target = new TestTimer();
            target.Tick(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTimer_MultiTick_Enumerable_ThrowsOnNullEnumerable()
        {
            var target = new TestTimer();
            target.Tick((IEnumerable<DateTimeOffset>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTimer_RequireStart_RequiresStart()
        {
            var target = new TestTimer(requireStart: true);
            target.Tick();  // Timer is not "started", requirestart is true, we should get an exception
        }

        [TestMethod]
        public void TestTimer_EnabledPropertyReflectsState()
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