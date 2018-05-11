using System;
using core;

namespace des
{
	public class SchedulerStopSimEvent : ISimEvent
	{
		public SchedulerStopSimEvent ()
		{
		}
		
		public void Entering(ISimEntity locale) {
			Scheduler.Instance().Stop();
		}
	}
}

