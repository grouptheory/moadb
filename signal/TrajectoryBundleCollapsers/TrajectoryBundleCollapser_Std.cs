using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryBundleCollapser_Std : ITrajectoryBundleCollapser
	{
		private static ITrajectoryBundleCollapser _instance;
		private static readonly string SUFFIX = "-Std";
		
		public static ITrajectoryBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new TrajectoryBundleCollapser_Std();
			}
			return _instance;
		}
		
		private TrajectoryBundleCollapser_Std ()
		{
		}
		
		public ITrajectory eval(ITrajectoryBundle tb) {
			
			ITrajectory mean = tb.MeanTrajectory;
			
			SortedList<double,double> alltimes = tb.Times;
			ITrajectory std = new Trajectory(tb.Name+SUFFIX, tb.TemporalGranularityThreshold, 0.0, 0.0);
			foreach (double t in alltimes.Keys) {
				double val = 0.0;
				double ct = 0.0;
				foreach (ITrajectory traj in tb.Trajectories) {
					double y = traj.eval(t);
					double ybar = mean.eval(t);
					val += (y-ybar)*(y-ybar);
					ct += 1.0;
				}
				std.add(t, Math.Sqrt(val/ct));
			}
			return std;
		}
	}
}
