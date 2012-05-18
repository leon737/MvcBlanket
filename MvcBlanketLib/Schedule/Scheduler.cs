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
using System.Threading;
using System.Diagnostics;

namespace MvcBlanketLib.Schedule
{
	public class Scheduler
	{
		private static Scheduler instance;
	    private IList<IScheduledTask> tasks;
	    private SchedulerSettings settings;

        private Scheduler(SchedulerSettings settings)
		{
			this.settings = settings;
            tasks = new List<IScheduledTask>();
			Initialize();
		}

        public static Scheduler Create(SchedulerSettings settings)
		{
            instance = new Scheduler(settings);
			return instance;
		}

        public static Scheduler Instance
		{
			get { return instance; }
		}

		void Initialize()
		{
            //timer = new Timer(TimerCb, null, 1000, Settings.PoolingInterval);
		}

        public void AddTask(IScheduledTask task)
        {
            
        }

        public void RemoveTask(IScheduledTask task)
        {
            throw new NotImplementedException();
        }

        public void RemoveDeprecatedTasks()
        {
            throw new NotImplementedException();
        }

	    public IEnumerable<IScheduledTask> Tasks
	    {
	        get
	        {
	            throw new NotImplementedException();
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
