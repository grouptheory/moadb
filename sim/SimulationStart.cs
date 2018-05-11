using System;
using core;

namespace sim
{
	public class SimulationStart : ISimulationStart
	{
		public IOrderbookEvent OrderbookEvent {
			get {return null;}
		}
		
		public override string ToString() {
			return "SimulationStart";
		}
	}
}

