using System;
using System.Collections.Generic;

namespace core
{
	public interface IPopulation : IEnumerable<IAgent>, ISimulationObserver
	{
		int Size {
			get;
		}
		
		IAgent getAgent(string name);
		IAgent getAgent(int i);
		void addAgent(IAgent ag);
		void removeAgent(IAgent ag);

		bool partition(IBlauSpace s, IBlauSpaceAxis axis, double myValue, int myId);

		IPopulation clone();
	}
}

