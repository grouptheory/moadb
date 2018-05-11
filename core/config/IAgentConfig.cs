using System;

namespace core
{
	public interface IAgentConfig
	{
		string AgentFactoryClassName {
			get;
		}
		
		IBlauSpaceConfig getBlauSpace();
	}
}

