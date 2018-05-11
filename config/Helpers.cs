using System;
using logger;
using core;
using blau;
using metrics;
using models;
using dist;
using sim;
using agent;
using orderbook;
using signal;

namespace config
{
	public class Helpers
	{
		static public AgentOrderbookLoader MakeAgentOrderbookLoader(string path) {
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(1, names, mins, maxs);
			IDistribution d = new Distribution_Gaussian(s, 0.0, 0.0);
			
			IAgentFactory afact = new AgentOrderbookLoader_Factory(path, d);
			IPopulation pop = PopulationFactory.Instance().create(afact, 1);
			
			AgentOrderbookLoader loader = null;
			foreach (IAgent ag in pop) {
				loader = (AgentOrderbookLoader)ag;
				break;
			}
			
			return loader;
		}
	}
}

