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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvcBlanketLib.Schedule
{
    public class Scheduler
    {
        private const int Jitter = 100;
        private static Scheduler instance;
        private IList<ScheduledTaskInternal> tasks;
        private Timer timer;
        private DateTime timerWillFire;

        private Scheduler()
        {
            tasks = new List<ScheduledTaskInternal>();
            Initialize();
        }

        public static Scheduler Create(SchedulerSettings settings)
        {
            instance = new Scheduler();
            return instance;
        }

        public static Scheduler Instance
        {
            get { return instance; }
        }

        void Initialize()
        {
            timerWillFire = DateTime.MaxValue;
        }

        public void AddTask(IScheduledTask task)
        {
            if (task.TaskAction == null) throw new ArgumentException("Task action cannot be null", "task");
            if (tasks.Any(t => t.Task == task)) throw new ArgumentException("Cannot add duplicated task", "task");
            tasks.Add(new ScheduledTaskInternal { Task = task, NextTimeToRun = GetNextTimeToRun(task), Enabled = true });
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            var nextTimeToRun = NextTimeToRun;
            if (nextTimeToRun == null) return;
            Debug.WriteLine("TWF: {0}  NTTR: {1}", timerWillFire.ToString("mm:ss:fff"), nextTimeToRun.Value.ToString("mm:ss:fff"));
            if (timerWillFire > nextTimeToRun.Value)
            {
                timerWillFire = nextTimeToRun.Value;
                if (timer != null)
                    timer.Dispose();
                TimeSpan runInterval = nextTimeToRun.Value - DateTime.UtcNow;
                Debug.WriteLine("RI: {0}", runInterval.TotalMilliseconds);
                if (runInterval < TimeSpan.Zero) runInterval = TimeSpan.Zero;
                timer = new Timer(TimerCb, null, runInterval, TimeSpan.Zero);
            }
        }

        private DateTime GetNextTimeToRun(IScheduledTask task)
        {
            switch (task.IntervalType)
            {
                case IntervalTypes.Never:
                case IntervalTypes.Once:
                    return task.StartTime;
                case IntervalTypes.Periodic:
                    return task.StartTime < DateTime.UtcNow - task.Interval ? DateTime.UtcNow + task.Interval : (task.StartTime <= DateTime.UtcNow ? task.StartTime + task.Interval : task.StartTime);
                default:
                    throw new ArgumentException("Invalid interval type given", "task");
            }
        }

        public void RemoveTask(IScheduledTask task)
        {
            if (!tasks.Any(t => t.Task == task)) throw new ArgumentException("Cannot find specified task to delete", "task");
            var taskToRemove = tasks.First(t => t.Task == task);
            tasks.Remove(taskToRemove);
        }

        public void RemoveDeprecatedTasks()
        {
            tasks = FutureTasks.ToList();
        }

        public IEnumerable<IScheduledTask> Tasks
        {
            get { return tasks.Select(t => t.Task); }
        }

        internal IEnumerable<ScheduledTaskInternal> FutureTasks
        {
            get
            {
                return tasks.Where(
                    t => t.Enabled && (
                    t.Task.IntervalType == IntervalTypes.Periodic ||
                    (t.Task.IntervalType == IntervalTypes.Once && t.Task.StartTime > DateTime.UtcNow)));
            }
        }

        internal DateTime? NextTimeToRun
        {
            get
            {
                if (!FutureTasks.Any()) return null;
                return FutureTasks.OrderBy(t => t.NextTimeToRun).Select(t => t.NextTimeToRun).First();
            }
        }

        private void TimerCb(object state)
        {
            var tasksToRun = TasksToRun;
            foreach (var task in tasksToRun)
            {
                ScheduledTaskInternal task1 = task;
                Task.Factory.StartNew(() => task1.Task.TaskAction());
                if (task.Task.IntervalType == IntervalTypes.Once)
                    task.Enabled = false;
                if (task.Task.IntervalType == IntervalTypes.Periodic)
                {
                    task.NextTimeToRun += task.Task.Interval;
                    Debug.WriteLine("Periodic NTTR {0:mm:ss:fff}", task.NextTimeToRun);
                }
            }
            timerWillFire = DateTime.MaxValue;
            UpdateTimer();
        }

        internal IEnumerable<ScheduledTaskInternal> TasksToRun
        {
            get
            {
                var possibleTasks =
                    tasks.Where(
                        (t => t.Enabled && (
                            (t.Task.IntervalType == IntervalTypes.Once && t.NextTimeToRun <= DateTime.UtcNow) ||
                              (t.Task.IntervalType == IntervalTypes.Periodic &&
                               Math.Abs((t.NextTimeToRun - DateTime.UtcNow).TotalMilliseconds) < Jitter))));
                return possibleTasks;
            }
        }

        //private void TimerCb(object state)
        //{
        //    foreach (var backgroundTask in Tasks)
        //    {
        //        if (backgroundTask.CheckInterval())
        //        {
        //            try
        //            {
        //                if (Monitor.TryEnter(backgroundTask))
        //                    backgroundTask.TaskAction();
        //            }
        //            catch (Exception ex)
        //            {
        //                var log = new EventLog();
        //                log.Source = "Application";
        //                log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
        //            }
        //            finally
        //            {
        //                Monitor.Exit(backgroundTask);
        //            }
        //        }
        //    }
        //}

    }


}
