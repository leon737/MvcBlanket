/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcBlanketLib.Schedule;

namespace MvcBlanketLibTest.ScheduleTests
{
    [TestClass]
    public class SchedulerTests
    {

        [TestMethod]
        public void CreateInstanceTest()
        {
            var settings = new SchedulerSettings();
            var scheduler = Scheduler.Create(settings);
            var instance = Scheduler.Instance;
            Assert.IsNotNull(scheduler);
            Assert.IsNotNull(instance);
            Assert.AreSame(scheduler, instance);
        }

        static Scheduler CreateScheduler()
        {
            var settings = new SchedulerSettings();
            return Scheduler.Create(settings);
        }


        [TestMethod]
        public void AddTaskTest()
        {
            var scheduler = CreateScheduler();
            var task = new ScheduledTask
                           {
                               TaskAction = () => { },
                               Interval = TimeSpan.FromMinutes(1)
                           };
            scheduler.AddTask(task);
            var tasks = scheduler.Tasks.ToList();
            Assert.AreEqual(1, tasks.Count());
            Assert.AreSame(task, tasks.First());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTaskWithoutActionTest()
        {
            var scheduler = CreateScheduler();
            var task = new ScheduledTask
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            scheduler.AddTask(task);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicatedTaskWithoutActionTest()
        {
            var scheduler = CreateScheduler();
            var task = new ScheduledTask
            {
                TaskAction = () => { },
                Interval = TimeSpan.FromMinutes(1)
            };
            scheduler.AddTask(task);
            scheduler.AddTask(task);
        }

        [TestMethod]
        public void RemoveTaskTest()
        {
            var scheduler = CreateScheduler();
            var task = new ScheduledTask
            {
                TaskAction = () => { },
                Interval = TimeSpan.FromMinutes(1)
            };
            scheduler.AddTask(task);
            scheduler.RemoveTask(task);
            var tasks = scheduler.Tasks.ToList();
            Assert.AreEqual(0, tasks.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveNotExistingTaskTest()
        {
            var scheduler = CreateScheduler();
            var task = new ScheduledTask
            {
                TaskAction = () => { },
                Interval = TimeSpan.FromMinutes(1)
            };
            scheduler.RemoveTask(task);
        }


        [TestMethod]
        public void RemoveDeprecatedTasksTest()
        {
            var scheduler = CreateScheduler();
            var periodicTask = new ScheduledTask
            {
                TaskAction = () => { },
                Interval = TimeSpan.FromMinutes(1),
                IntervalType = IntervalTypes.Periodic
            };
            scheduler.AddTask(periodicTask);
            var futureTask = new ScheduledTask
            {
                TaskAction = () => { },
                StartTime = DateTime.UtcNow.AddMinutes(1),
                IntervalType = IntervalTypes.Once
            };
            scheduler.AddTask(futureTask);
            var pastTask = new ScheduledTask
            {
                TaskAction = () => { },
                StartTime = DateTime.UtcNow.AddMinutes(-1),
                IntervalType = IntervalTypes.Once
            };
            scheduler.AddTask(pastTask);
            scheduler.RemoveDeprecatedTasks();
            var tasks = scheduler.Tasks.ToList();
            Assert.AreEqual(2, tasks.Count());
            Assert.AreSame(periodicTask, tasks.First());
            Assert.AreSame(futureTask, tasks.Last());
        }

        [TestMethod]
        public void NextTimeToRunForPeriodicTasksTest()
        {
            var scheduler = CreateScheduler();
            scheduler.AddTask(
                new ScheduledTask
                    {
                        TaskAction = () => { },
                        Interval = TimeSpan.FromMinutes(1),
                        IntervalType = IntervalTypes.Periodic
                    });
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () => { },
                    Interval = TimeSpan.FromMinutes(2),
                    IntervalType = IntervalTypes.Periodic
                });
            var nextTimeToRun = scheduler.NextTimeToRun;
            Assert.IsNotNull(nextTimeToRun);
            if (nextTimeToRun.Value > DateTime.UtcNow.AddMinutes(1))
                Assert.Fail();
        }

        [TestMethod]
        public void NextTimeToRunForOnceTasksTest()
        {
            var scheduler = CreateScheduler();
            var startTime1 = DateTime.UtcNow.AddDays(2);
            var startTime2 = DateTime.UtcNow.AddDays(1);
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () => { },
                    StartTime = startTime1,
                    IntervalType = IntervalTypes.Once
                });
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () => { },
                    StartTime = startTime2,
                    IntervalType = IntervalTypes.Once
                });
            var nextTimeToRun = scheduler.NextTimeToRun;
            Assert.IsNotNull(nextTimeToRun);
            Assert.AreEqual(startTime2, nextTimeToRun.Value);
        }

        [TestMethod]
        public void NextTimeToRunForNoMoreTasksToRunTest()
        {
            var scheduler = CreateScheduler();
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () => { },
                    StartTime = DateTime.UtcNow.AddMinutes(-1),
                    IntervalType = IntervalTypes.Once
                });
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () => { },
                    StartTime = DateTime.UtcNow.AddMinutes(1),
                    IntervalType = IntervalTypes.Never
                });
            var nextTimeToRun = scheduler.NextTimeToRun;
            Assert.IsNull(nextTimeToRun);
        }

        [TestMethod]
        public void TestPeriodicTaskCall5Times()
        {
            DateTime prevRun = DateTime.UtcNow;
            List<TimeSpan> intervals = new List<TimeSpan>();
            int calls = 0;
            var scheduler = CreateScheduler();
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () =>
                    {
                        calls++;
                        TimeSpan interval = DateTime.UtcNow - prevRun;
                        prevRun = DateTime.UtcNow;
                        intervals.Add(interval);
                    },
                    IntervalType = IntervalTypes.Periodic,
                    Interval = TimeSpan.FromMilliseconds(100)
                });
            Thread.Sleep(600);
            double averageInterval = intervals.Average(s => s.TotalMilliseconds);
            if (Math.Abs(100 - averageInterval) > 10) Assert.Fail("Average interval is " + averageInterval);
        }

        [TestMethod]
        public void TestOnceTaskCall()
        {
            DateTime callWasMade = DateTime.MinValue;
            DateTime startTime = DateTime.UtcNow.AddSeconds(1);
            var scheduler = CreateScheduler();
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () =>
                                     {
                                         callWasMade = DateTime.UtcNow;
                                     },
                    IntervalType = IntervalTypes.Once,
                    StartTime = startTime
                });
            Thread.Sleep(1500);
            if (Math.Abs((startTime - callWasMade).TotalMilliseconds) > 100) Assert.Fail("Call was made at wrong time");
        }

        [TestMethod]
        public void TestNeverTaskCall()
        {
            bool callWasMade = false;
            DateTime startTime = DateTime.UtcNow;
            var scheduler = CreateScheduler();
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () =>
                    {
                        callWasMade = true;
                    },
                    IntervalType = IntervalTypes.Never,
                    StartTime = startTime
                });
            Thread.Sleep(500);
            Assert.IsFalse(callWasMade);
        }

        [TestMethod]
        public void TestPeriodicTaskWithEndTimeInThePastCall()
        {
            bool callWasMade = false;
            var scheduler = CreateScheduler();
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () =>
                    {
                        callWasMade = true;
                    },
                    IntervalType = IntervalTypes.Periodic,
                    EndTime = DateTime.UtcNow.AddDays(-1),
                    Interval = TimeSpan.FromMilliseconds(100)
                });
            Thread.Sleep(500);
            Assert.IsFalse(callWasMade);
        }

        [TestMethod]
        public void TestOnceTaskWithEndTimeInThePastCall()
        {
            bool callWasMade = false;
            var scheduler = CreateScheduler();
            scheduler.AddTask(
                new ScheduledTask
                {
                    TaskAction = () =>
                    {
                        callWasMade = true;
                    },
                    IntervalType = IntervalTypes.Once,
                    StartTime = DateTime.UtcNow.AddMilliseconds(100),
                    EndTime = DateTime.UtcNow.AddDays(-1)
                });
            Thread.Sleep(500);
            Assert.IsTrue(callWasMade); // because EndTime for Once tasks is not taken into the account
        }

    }
}
