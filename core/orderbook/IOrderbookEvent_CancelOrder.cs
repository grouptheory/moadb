using System;

namespace core
{
	public interface IOrderbookEvent_CancelOrder : IOrderbookEvent
	{
		IOrder_Mutable getOrder();
	}
}
