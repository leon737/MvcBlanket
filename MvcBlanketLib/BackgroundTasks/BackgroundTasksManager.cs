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
