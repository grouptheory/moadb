using System;
using System.Collections.Generic;

namespace core
{
	public interface ISimulationParameters
	{
		IPopulation Population {
			get;
		}
		
		IOrderbook_Agent Orderbook {
			get;
		}
		
		double StartTime {
			get;
		}
		
		double Duration {
			get;
		}
		
		double Time {
			get;
		}
	}
}

