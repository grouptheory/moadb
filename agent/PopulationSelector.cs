using System;
using System.Collections.Generic;
using core;
using logger;

namespace agent
{
	/// <summary>
    /// PopulationSelector class
    /// </summary>
	public class PopulationSelector
	{
		// select one agent from a population, at random
		public static IAgent Select(IPopulation pop) {
			if (pop.Size < 1) {
				throw new Exception("PopulationSelector cannot Select from an empty Population!");
			}
			
			int index = SingletonRandomGenerator.Instance.Next(0, pop.Size-1);
			IAgent ag = pop.getAgent (index);
			
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(PopulationSelector), "PopulationSelector selected "+ag.ToString());
			
			return ag;
		}
		
		// select n agents from a population, at random
		public static IList<IAgent> Select(IPopulation pop, int n) {
			
			if (n > pop.Size) {
				throw new Exception("PopulationSelector cannot Select more Agents than are present in the Population!");
			}
			
			IList<IAgent> list = new List<IAgent>();
			
			if (n > pop.Size/2) {
				// select complement if asked for too many
				IList<IAgent> not_selected = Select(pop,  pop.Size-n);
				foreach (IAgent ag in pop) {
					if ( ! not_selected.Contains(ag)) {
						list.Add (ag);
					}
				}
			}
			else {
				// select one at a time
				while (list.Count < n) {
					IAgent ag = Select (pop);
					if ( ! list.Contains(ag)) {
						list.Add(ag);
					}
				}
			}
			
			return list;
		}
	}
}

