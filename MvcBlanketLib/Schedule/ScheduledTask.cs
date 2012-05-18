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

namespace MvcBlanketLib.Schedule
{
	public class ScheduledTask : IScheduledTask
	{
		public Action TaskAction { get; set; }
	    public TimeSpan Interval { get; set; }
	    public DateTime StartTime { get; set; }
	    public IntervalTypes IntervalType { get; set; }


	    //private int interval;
        //public int Interval { get { return interval; } set { interval = value; ResetTicks(); } }

        //private int remainingTicks;

        //public ScheduledTask()
        //{
        //    ResetTicks();
        //}

        //void ResetTicks ()
        //{
        //    remainingTicks = Interval;
        //}

        //public bool CheckInterval()
        //{
        //    remainingTicks--;
        //    if (remainingTicks == 0)
        //    {
        //        ResetTicks();
        //        return true;
        //    }
        //    return false;
        //}
	}

    public interface IScheduledTask
    {
        Action TaskAction { get; set; }

        TimeSpan Interval { get; set; }

        DateTime StartTime { get; set; }

        IntervalTypes IntervalType { get; set; }
    }

    public enum IntervalTypes
    {
        Never,
        Once,
        Periodic
    }
}
