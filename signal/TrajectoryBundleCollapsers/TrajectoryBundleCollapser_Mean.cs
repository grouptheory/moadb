using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryBundleCollapser_Mean : ITrajectoryBundleCollapser
	{
		private static ITrajectoryBundleCollapser _instance;
		private static readonly string SUFFIX = "-Mean";
		
		public static ITrajectoryBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new TrajectoryBundleCollapser_Mean();
			}
			return _instance;
		}
		
		private TrajectoryBundleCollapser_Mean ()
		{
		}
		
		public ITrajectory eval(ITrajectoryBundle tb) {
			SortedList<double,double> alltimes = tb.Times;
			
			ITrajectory mean = new Trajectory(tb.Name+SUFFIX, tb.TemporalGranularityThreshold, 0.0, 0.0);
			foreach (double t in alltimes.Keys) {
				double val = 0.0;
				double ct = 0.0;
				foreach (ITrajectory traj in tb.Trajectories) {
					val += traj.eval(t);
					ct += 1.0;
				}
				mean.add(t, val/ct);
			}
			return mean;
		}
	}
}

