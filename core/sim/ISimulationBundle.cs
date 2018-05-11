using System;
using System.Collections.Generic;

namespace core
{
	public interface ISimulationBundle : ISimulationParameters
	{
		ISimulationResultsBundle run(int trials);
		
		void add(IPassiveTrajectoryFactory tf);
		void add(IAgentEvaluationFactory evf, IAgentEvaluationConfig conf);
		
		IAgentEvaluationConfig getAgentEvaluationConfig(IAgentEvaluationFactory evf);
		
		IEnumerable<IPassiveTrajectoryFactory> getPassiveTrajectoryFactories();
		IEnumerable<IAgentEvaluationFactory> getAgentEvaluationFactories();
		
		IBlauSpaceLattice getLattice(IDistribution d, IAgentEvaluationBundle aeb);
	}
}

