using System;

namespace core
{
	public interface ITrajectoryFactoryConfig
	{	
		string Name {
			get;
		}
		double MinGranularity {
			get;
		}
		double HistoryCoefficient {
			get;
		}
	}
}

