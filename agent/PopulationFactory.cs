using System;
using core;
using logger;

namespace agent
{
	/// <summary>
    /// PopulationFactory class
    /// </summary>
	public class PopulationFactory : IPopulationFactory
	{
		// singleton instance
		private static IPopulationFactory _instance;
		public static IPopulationFactory Instance() {
			if (_instance == null) {
				_instance = new PopulationFactory();
			}
			return _instance;
		}
		
		// private constructor
		private PopulationFactory () { }
		
		// use an agentfactory to create the population
		public IPopulation create(IAgentFactory afact, int size) {
			IPopulation pop = new Population();
			
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(PopulationFactory), "PopulationFactory creating "+size+" agents using IAgentFactory = "+afact.ToString());

			afact.Distribution.setTotalSamples(size);

			for (int i=0; i<size; i++) {
				IAgent ag = afact.create();
				if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(PopulationFactory), "PopulationFactory created Agent # "+i+" : "+ag);
				pop.addAgent(ag);
			}

			return pop;
		}
		
		// diagnostic to string
		public override string ToString ()
		{
			string s = "PopulationFactory";
			return s;
		}
	}
}

