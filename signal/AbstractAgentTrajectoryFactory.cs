using System;
using core;

namespace signal
{
	public abstract class AbstractAgentTrajectoryFactory : AbstractPassiveTrajectoryFactory
	{
		
		public override sealed void SimulationStartNotification() {
			_agent = Population.getAgent(_agent.GetName());
		}

		protected IAgent _agent;
		
		protected AbstractAgentTrajectoryFactory(IAgent agent, double timeQuantum, double historicalBias) : base(timeQuantum, historicalBias) {
			_agent = agent;
			reset();
		}
	}
}

