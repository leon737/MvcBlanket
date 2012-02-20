using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBlanketLib.BackgroundTasks
{
	public class BackgroundTask
	{
		public Action TaskAction { get; set; }
		private int interval;
		public int Interval { get { return interval; } set { interval = value; ResetTicks(); } }

		private int remainingTicks;

		public BackgroundTask()
		{
			ResetTicks();
		}

		void ResetTicks ()
		{
			remainingTicks = Interval;
		}

		public bool CheckInterval()
		{
			remainingTicks--;
			if (remainingTicks == 0)
			{
				ResetTicks();
				return true;
			}
			return false;
		}
	}
}
