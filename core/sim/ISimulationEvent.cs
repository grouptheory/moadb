using System;
using core;

namespace core
{
	public interface ISimulationEvent 
	{
		IOrderbookEvent OrderbookEvent {
			get;
		}
	}
}

