using System;
using core;

namespace orderbook
{
	// whenever a Big or Ask is added,
	// this event is wrapped in a NotifyMatcher SimEvent 
	// which is then sent to the Matcher via the DES.
	// Upon arriving there, the Matcher's recvNotification
	// is called.
	// 
	
	public class OrderbookEvent_AddOrder : IOrderbookEvent_AddOrder
	{
		private IOrder_Mutable _order;
		
		public OrderbookEvent_AddOrder(IOrder_Mutable order)
		{
			_order = order;
		}
		
		public IOrder_Mutable getOrder() {
			return _order;
		}
		
		public override string ToString() {
			return "OrderbookEvent_AddOrder: "+_order;
		}
	}
}

