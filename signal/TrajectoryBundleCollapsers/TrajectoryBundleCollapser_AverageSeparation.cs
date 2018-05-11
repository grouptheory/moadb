using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryBundleCollapser_AverageSeparation : ITrajectoryBundleCollapser
	{	
		private static ITrajectoryBundleCollapser _instance;
		private static readonly string SUFFIX = "-AveSep";
		
		public static ITrajectoryBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new TrajectoryBundleCollapser_AverageSeparation();
			}
			return _instance;
		}
		
		private TrajectoryBundleCollapser_AverageSeparation()
		{
		}
		
		public ITrajectory eval(ITrajectoryBundle tb) {
			
			SortedList<double,double> alltimes = tb.Times;
			
			ITrajectory avesep = new Trajectory(tb.Name+SUFFIX, tb.TemporalGranularityThreshold, 0.0, 0.0);
			foreach (double t in alltimes.Keys) {
				double val = 0.0;
				double ct = 0.0;

				foreach (ITrajectory traj1 in tb.Trajectories) {
					if (traj1.Times.Count == 0) continue;

					foreach (ITrajectory traj2 in tb.Trajectories) {
						if (traj1==traj2) continue;
						if (traj2.Times.Count == 0) continue;

						val += Math.Abs(traj1.eval(t) - traj2.eval(t));
						ct += 1.0;
					}
				}
				avesep.add(t, val/ct);
			}
			return avesep;
		}
	}
}

