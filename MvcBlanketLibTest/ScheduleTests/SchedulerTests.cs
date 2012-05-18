using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcBlanketLib.Schedule;

namespace MvcBlanketLibTest.ScheduleTests
{
    [TestClass]
    public class SchedulerTests
    {
        public SchedulerTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }


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

        Scheduler CreateScheduler ()
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
                               TaskAction = () => {},
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
            var tasks = scheduler.Tasks.ToList();
            scheduler.RemoveTask(task);
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
                StartTime = DateTime.Now.AddMinutes(1),
                IntervalType = IntervalTypes.Once
            };
            scheduler.AddTask(futureTask);
            var pastTask = new ScheduledTask
            {
                TaskAction = () => { },
                StartTime = DateTime.Now.AddMinutes(-1),
                IntervalType = IntervalTypes.Once
            };
            scheduler.AddTask(pastTask);
            var tasks = scheduler.Tasks.ToList();
            scheduler.RemoveDeprecatedTasks();
            Assert.AreEqual(2, tasks.Count());
            Assert.AreSame(periodicTask, tasks.First());
            Assert.AreSame(futureTask, tasks.Last());
        }
    }
}
