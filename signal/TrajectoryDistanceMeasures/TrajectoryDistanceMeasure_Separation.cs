using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryDistanceMeasure_Separation : ITrajectoryDistanceMeasure
	{
		public TrajectoryDistanceMeasure_Separation ()
		{
		}
		
		private SortedList<double,double> getTimes(ITrajectory t1, ITrajectory t2) {
			SortedList<double,double> alltimes = new SortedList<double,double>();
			foreach (double t in t1.Times) {
				if ( ! alltimes.ContainsKey(t)) {
					alltimes.Add(t,t);
				}
			}
			foreach (double t in t2.Times) {
				if ( ! alltimes.ContainsKey(t)) {
					alltimes.Add(t,t);
				}
			}
			return alltimes;
		}
		
		public ITrajectory eval(ITrajectory traj1, ITrajectory traj2){
			SortedList<double,double> alltimes = getTimes(traj1, traj2);
			
			ITrajectory sep = new Trajectory(traj1.Name, 0.0, 0.0, 0.0);
			foreach (double t in alltimes.Keys) {
				double val = eval (traj1, traj2, t);
				sep.add(t, val);
			}
			return sep;
		}
		
		public double eval(ITrajectory traj1, ITrajectory traj2, double t) {
			return Math.Abs(traj1.eval(t) - traj2.eval(t));
		}
	}
}

