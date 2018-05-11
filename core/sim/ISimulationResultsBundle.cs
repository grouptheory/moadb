using System;
using System.Collections.Generic;

namespace core
{
	public interface ISimulationResultsBundle
	{
		bool Valid {
			get;
		}
		
		void add(ISimulationResults res);
		void add(ISimulationResultsBundle resb);
		
		IEnumerable<ITrajectoryBundle> getTrajectoryBundles();
		IEnumerable<IAgentEvaluationBundle> getAgentEvaluationBundles();
		
		string ToStringLong ();
	}
}

