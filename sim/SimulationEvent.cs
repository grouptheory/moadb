using System;
using core;

namespace sim
{
	public class SimulationEvent : ISimulationEvent 
	{
		private IOrderbookEvent _evt;
		
		public SimulationEvent(IOrderbookEvent evt) {
			_evt = evt;
		}
		
		public IOrderbookEvent OrderbookEvent {
			get { return _evt; }
		}
	}
}

