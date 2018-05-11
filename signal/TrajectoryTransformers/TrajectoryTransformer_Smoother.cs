using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryTransformer_Smoother : ITrajectoryTransformer
	{
		public ITrajectory eval(ITrajectory orig) {
			
			ITrajectory smooth = new Trajectory(orig.Name+"-smooth("+_window+")", 0.0, 0.0, 0.0);
			foreach (double t in orig.Times) {
				double val = 0.0;
				double ct = 0.0;
				for (double avet = t-_window; avet<=t+_window; avet+=_windowstep) {
					val += orig.eval(avet);
					ct += 1.0;
				}
				smooth.add(t, val/ct);
			}
			return smooth;
		}
		
		private double _window;
		private double _windowstep;
		
		public TrajectoryTransformer_Smoother (double window, int steps)
		{
			_window = window;
			_windowstep = 2.0 * window / (double)steps;
		}
	}
}

