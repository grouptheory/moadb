using System;
using System.Collections.Generic;

namespace core
{
	public interface ITrajectory : INamedTemporalObject
	{	
		bool Valid {
			get;
		}
		
		double TemporalGranularityThreshold {
			get;
		}
		
		double eval(double time);
		void add(double time, double val);
		
		bool ThresholdTimePassed(double time);
		
		IList<double> Times {
			get;
		}
		
		string ToStringLong ();
	}
}

