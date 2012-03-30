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
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MvcBlanketLib.BackgroundTasks
{
	public class BackgroundTasksManager
	{
		private static BackgroundTasksManager instance;
		public IList<BackgroundTask> Tasks { get; private set; }
		public BackgroundTasksManagerSettings Settings { get; private set; }
		private Timer timer;

		private BackgroundTasksManager(BackgroundTasksManagerSettings settings)
		{
			Settings = settings;
			Tasks = new List<BackgroundTask>();
			Initialize();
		}

		public static BackgroundTasksManager Create(BackgroundTasksManagerSettings settings)
		{
			instance = new BackgroundTasksManager(settings);
			return instance;
		}

		public static BackgroundTasksManager Instance
		{
			get { return instance; }
		}

		void Initialize()
		{
			timer = new Timer(TimerCb, null, 1000, Settings.PoolingInterval);
		}

		private void TimerCb(object state)
		{
			foreach (var backgroundTask in Tasks)
			{
				if (backgroundTask.CheckInterval())
				{
					try
					{
						if (Monitor.TryEnter(backgroundTask))
							backgroundTask.TaskAction();
					}
					catch (Exception ex)
					{
						var log = new EventLog();
						log.Source = "Application";
						log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
					}
					finally
					{
						Monitor.Exit(backgroundTask);
					}
				}
			}
		}

	}


}
