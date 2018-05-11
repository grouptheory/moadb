using System;

namespace core
{
	public interface IPopulationFactory
	{
		IPopulation create(IAgentFactory afact, int size);
	}
}

