using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryBundleCollapser_CentralDTW : ITrajectoryBundleCollapser
	{
		private static ITrajectoryBundleCollapser _instance;
		public static readonly string SUFFIX = "-CentralDTW";
		
		public static ITrajectoryBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new TrajectoryBundleCollapser_CentralDTW();
			}
			return _instance;
		}
		
		private TrajectoryBundleCollapser_CentralDTW ()
		{
		}
		
		public ITrajectory eval(ITrajectoryBundle tb) {
			
			TrajectoryDistanceMeasure_DTW dist = new TrajectoryDistanceMeasure_DTW();
			
			ITrajectory winner = null;

			double winningD = Double.PositiveInfinity;
			foreach (ITrajectory t in tb.Trajectories) {
				if (winner==null) winner = t;

				if (t.Times.Count == 0) continue;

				double totalD = 0.0;
				foreach (ITrajectory t2 in tb.Trajectories) {
					if (t==t2) continue;
					if (t2.Times.Count == 0) continue;

					double minDur = Math.Min (t.MaximumTime, t2.MaximumTime);
					double inc = dist.eval(t, t2, minDur);
					totalD += Math.Abs(inc);
					
					// Console.WriteLine("d("+t.GetHashCode()+" -- "+t2.GetHashCode()+")  =  "+inc);
				}
				if (totalD < winningD) {
					winningD = totalD;
					winner = t;
				}
				
				// Console.WriteLine("*** total d("+t.GetHashCode()+")  =  "+totalD);
			}
			
			/*
			ITrajectory CentralTrajectory = new Trajectory(tb.Name+SUFFIX, tb.TemporalGranularityThreshold, 0.0);
			foreach (double t in winner.Times) {
				CentralTrajectory.add(t, winner.eval (t));
			}
			*/
				
			return winner;
		}
	}
}

