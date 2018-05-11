using System;
using System.Collections.Generic;

namespace core
{
	public interface IAgent : ISimulationObserver, IOrderOwner, ISimEntity
	{
		int ID {
			get;
		}
		
		IAgentFactory Creator {
			get;
		}
		
		IBlauPoint Coordinates {
			get;
		}
				
		IAgent clone();
		
		Dictionary<string,double>.KeyCollection GetMetrics();
		double GetMetricValue(string metricName);
		void SetMetricValue(string metricName, double val);
		bool HasMetric(string metricName);
		
		int NumBids {
			get;
		}
		
		int NumAsks {
			get;
		}
	}
}

