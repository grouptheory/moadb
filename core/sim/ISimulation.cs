using System;
using System.Collections.Generic;

namespace core
{
	public interface ISimulation : ISimEntity, ISimulationParameters, IOrderbookObserver
	{
		ISimulationResults run();
		
		void add(IPassiveTrajectoryFactory tf);
		void add(IAgentEvaluationFactory evf);
		
		IEnumerator<IPassiveTrajectoryFactory> getPassiveTrajectoryFactories();
		IEnumerator<IAgentEvaluationFactory> getAgentEvaluationFactories();	
		
		void broadcast(ISimulationEvent se);
	}
}

