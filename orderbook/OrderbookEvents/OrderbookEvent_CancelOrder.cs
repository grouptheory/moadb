using System;
using core;

namespace orderbook
{
	// whenever an order is cancelled
	// this event is wrapped in a NotifyObserver SimEvent 
	// which is then sent to the Observer via the DES.
	// Upon arriving there, the Observer's recvNotification
	// is called.
	// 
	
	public class OrderbookEvent_CancelOrder : IOrderbookEvent_CancelOrder
	{
		private IOrder_Mutable _order;
		
		public OrderbookEvent_CancelOrder(IOrder_Mutable order)
		{
			_order = order;
		}
		
		public IOrder_Mutable getOrder() {
			return _order;
		}
		
		public override string ToString() {
			return "OrderbookEvent_CancelOrder: "+_order;
		}
	}
}

