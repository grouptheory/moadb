using System;
using core;

namespace sim
{
	public class SimulationEnd : ISimulationEnd
	{
		public IOrderbookEvent OrderbookEvent {
			get {return null;}
		}
		
		public override string ToString() {
			return "SimulationEnd";
		}
	}
}

