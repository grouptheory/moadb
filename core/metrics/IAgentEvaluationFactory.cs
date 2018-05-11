using System;

namespace core
{
	public interface IAgentEvaluationFactory : INamedObject, ISimulationObserver
	{
		IAgentEvaluation create();
	}
}

