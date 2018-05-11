using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryDistanceMeasure_DTW
	{
		
		private static double WARP_TIME_SECONDS = 60.0;
		public double MaximumWarpTime {
			get { return WARP_TIME_SECONDS; }
			set { WARP_TIME_SECONDS = value; }
		}
		
		private static int TIME_GRID = 100;
		public int NumberGridIntervals {
			get { return TIME_GRID; }
			set { TIME_GRID = value; }
		}
		
		public TrajectoryDistanceMeasure_DTW ()
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
			if ((_traj1 != traj1) || (_traj2 != traj2)) {
				double dt1 = (traj1.MaximumTime-traj1.MinimumTime)/(double)TIME_GRID;
				double dt2 = (traj2.MaximumTime-traj2.MinimumTime)/(double)TIME_GRID;
				_dt = Math.Min (dt1, dt2);
				int w = (int)(WARP_TIME_SECONDS / _dt);
				computeDTWArray(traj1, traj2, TIME_GRID, w);
			}
			Trajectory traj = new Trajectory(traj1.Name+"-"+traj2.Name, 0.0, 0.0, 0.0);
			int i = TIME_GRID-1;
			int j = TIME_GRID-1;
			while ((i != 0) || (j != 0)) {
				traj.add(_dt*(double)i, traj1.eval(_dt*(double)i)- traj2.eval (_dt*(double)j));
				if (_move[i,j]=='x')  i=i-1;
				if (_move[i,j]=='y')  j=j-1;
				if (_move[i,j]=='b') {i=i-1;j=j-1;}
			}
			return traj;
		}
		
		public double eval(ITrajectory traj1, ITrajectory traj2, double t) {
			if ((_traj1 != traj1) || (_traj2 != traj2)) {
				double dt1 = (traj1.MaximumTime-traj1.MinimumTime)/(double)TIME_GRID;
				double dt2 = (traj2.MaximumTime-traj2.MinimumTime)/(double)TIME_GRID;
				_dt = Math.Min (dt1, dt2);
				int w = (int)(WARP_TIME_SECONDS / _dt);
				computeDTWArray(traj1, traj2, TIME_GRID, w);
			}
			int index1 = (int)(t/_dt);
			int i = TIME_GRID-1;
			int j = TIME_GRID-1;
			while (i > index1) {
				if (_move[i,j]=='x')  i=i-1;
				if (_move[i,j]=='y')  j=j-1;
				if (_move[i,j]=='b') {i=i-1;j=j-1;}
			}
			return traj1.eval (_dt*(double)i) - traj2.eval (_dt*(double)j);
		}
		
		private ITrajectory _traj1, _traj2;
		private char [,] _move;
		private double _dt;
		
		private double [] TrajectoryToArray(ITrajectory traj1, int n) {
			double [] val = new double [n];
			int i=0;
			for (double t=traj1.MinimumTime; t<traj1.MaximumTime; t+= _dt) {
				val[i] = traj1.eval(t);
				i++;
				if (i >= n) break;
			}
			return val;
		}
		
		private void computeDTWArray(ITrajectory traj1, ITrajectory traj2, int n, int w) {
			double [] s = TrajectoryToArray(traj1, n);
			double [] t = TrajectoryToArray(traj2, n);
			DTWDistance(s, t, w);
			_traj1 = traj1;
			_traj2 = traj2;
		}
		
		private double DTWDistance(double[] s, double[] t, int w) {
		    double [,] DTW = new double [s.Length,t.Length];
		    _move = new char [s.Length,t.Length];
			
			double cost;
		
		    w = Math.Max(w, Math.Abs(s.Length - t.Length)); // adapt window size (*)
		
		    for (int i = 0; i < s.Length; i++) {
				for (int j = 0; j < t.Length; j++) {
		    		DTW[i,j] = Double.PositiveInfinity;
				}
			}
		    DTW[0,0] = 0.0;
		
		    for (int i = 1; i < s.Length; i++) {
				for (int j = Math.Max(1, i-w); j < Math.Min(t.Length, i+w); j++) {
					
					// Console.WriteLine("i="+i+" -- j="+j);
					
		            cost = d(s[i], t[j]);
					double inc = Math.Min(
						                        Math.Min(DTW[i-1,j],    // insertion
		                                                 DTW[i,j-1]),   // deletion
		                                        DTW[i-1,j-1]);          // match
					
					if (inc == DTW[i-1,j]) { _move[i,j] = 'x'; }
					else if (inc == DTW[i,j-1]) { _move[i,j] = 'y'; }
					else { _move[i,j] = 'b'; }
					
		            DTW[i,j] = cost + inc;
				}
			}
		    return DTW[s.Length-1,t.Length-1];
		}
				
		private double d(double sval, double tval) {
			return Math.Abs(sval-tval);
		}
	}
}

