using System;
using System.Collections.Generic;

namespace core
{
	public interface IOrderbook_Matcher : IOrderbook_Agent
	{
		IDictionary<double, IList<IOrder_Mutable>> getBids_Mutable();
		IDictionary<double, IList<IOrder_Mutable>> getAsks_Mutable();
		
		bool flushOrder(IOrder_Mutable order);
		
		bool notifyObserverSynchronous(IOrderbookEvent evt);

		IMatcher getMatcher();
		IList<IOrderbookObserver> getObserver();
	}
}

