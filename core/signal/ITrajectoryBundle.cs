using System;
using System.Collections.Generic;

namespace core
{
	public interface ITrajectoryBundle : INamedTemporalObject, IPresentable
	{
		bool Valid {
			get;
		}
		
		ITrajectory MeanTrajectory {
			get;
		}
		
		ITrajectory StdTrajectory {
			get;
		}
		
		ITrajectory CentralTrajectory {
			get;
		}
		
		ITrajectory CentralDevTrajectory {
			get;
		}
		
		bool addTrajectory(ITrajectory trajectory);
		
		bool Empty {
			get;
		}
		
		double TemporalGranularityThreshold {
			get;
		}
		
		SortedList<double,double> Times {
			get;
		}
		
		List<ITrajectory> Trajectories {
			get;
		}
		
		string ToStringLong ();
	}
}

