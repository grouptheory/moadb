using System;
using System.Collections.Generic;

namespace core
{
	public interface ISimulationResults
	{
		bool Valid {
			get;
		}
		
		void add(ITrajectory t);
		void add(IAgentEvaluation a);
		
		IEnumerable<ITrajectory> getTrajectories();
		IEnumerable<IAgentEvaluation> getAgentEvaluations();
		
		string ToStringLong ();
	}
}

