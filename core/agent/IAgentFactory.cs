using System;

namespace core
{
	public interface IAgentFactory
	{
		void Initialize(ITableGenerationConfig config);
		
		IDistribution Distribution {
			get;
		}
		
		IAgent create();
		
		IAgent cloneAgent(IAgent orig);
	}
}

