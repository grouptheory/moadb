using System;
using core;

namespace orderbook
{
	// whenever the Matcher fills all or part of an order in the OB
	// it asks the OB to notify its observer (synchronously)
	// using the notifyObserver method via this event.
	// 
	public class OrderbookEvent_FillOrder : IOrderbookEvent_FillOrder
	{
		private IOrder_Mutable _order;
		private double _executionPrice;
		private int _volume;
		private bool _filled;
		
		public OrderbookEvent_FillOrder(IOrder_Mutable order, double executionPrice, int volume, bool filled)
		{
			_order = order;
			_executionPrice = executionPrice;
			_volume = volume;
			_filled = filled;
		}
		
		public IOrder_Mutable getOrder() {
			return _order;
		}
		
		public double getExecutionPrice() {
			return _executionPrice;
		}
		
		public int getVolume() {
			return _volume;
		}
		
		public bool orderFilled() {
			return _filled;
		}
		
		public override string ToString() {
			return "OrderbookEvent_FillOrder: "+_order;
		}
	}
}

