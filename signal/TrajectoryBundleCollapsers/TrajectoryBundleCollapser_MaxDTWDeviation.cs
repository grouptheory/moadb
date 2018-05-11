using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryBundleCollapser_MaxDTWDeviation : ITrajectoryBundleCollapser
	{
		private static ITrajectoryBundleCollapser _instance;
		private static readonly string SUFFIX = "-MaxDTWDeviation";
		
		public static ITrajectoryBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new TrajectoryBundleCollapser_MaxDTWDeviation();
			}
			return _instance;
		}
		
		private TrajectoryBundleCollapser_MaxDTWDeviation ()
		{
		}
		
		public ITrajectory eval(ITrajectoryBundle tb) {
			
			ITrajectory central = tb.CentralTrajectory;
						
			ITrajectory dev = new Trajectory(tb.Name+SUFFIX, tb.TemporalGranularityThreshold, 0.0, 0.0);
			foreach (double t in central.Times) {
				double max = Double.NegativeInfinity;
				double centralval = central.eval(t);
					
				foreach (ITrajectory traj in tb.Trajectories) {
					if (traj==central) continue;
					if (traj.Times.Count == 0) continue;

					double y = traj.eval(t);
					double diff = Math.Abs (y-centralval);
					if (diff > max) max=diff;
				}
				
				dev.add(t, max);
			}
			
			return dev;
		}
	}
}

