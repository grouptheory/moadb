using System;

namespace core
{
	public interface ITableGenerationConfig
	{
		string AgentFactoryClassName {
			get;
		}
		
		int NumAgents {
			get;
		}
		
		double DurationHours {
			get;
		}
		
		int Populations {
			get;
		}
		
		int Trials {
			get;
		}
		
		int NumCombs {
			get;
		}
		
		double Lambda {
			get;
		}
		
		double Gamma {
			get;
		}
		
		string InitialOrderbook {
			get;
		}
		
		double InitialBurninHours {
			get;
		}
	}
}

