using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryTransformer_UniformizingSampler
	{
		public ITrajectory eval(ITrajectory orig) {
			double finish = orig.MaximumTime;
			double start = orig.MinimumTime;
				
			ITrajectory X = new Trajectory(orig.Name+"-Uniformized("+_timeStep+")", 0.0, 0.0, 0.0);
			for (double t = start; t <= finish; t+= _timeStep) {
				double Xt = orig.eval(t);
				X.add(t, Xt);
			}
			
			return X;
		}
		
		private double _timeStep;
		
		public TrajectoryTransformer_UniformizingSampler(double timeStep)
		{
			_timeStep = timeStep;
		}
	}
}

