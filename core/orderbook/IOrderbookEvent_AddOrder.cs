using System;

namespace core
{
	public interface IOrderbookEvent_AddOrder : IOrderbookEvent
	{
		IOrder_Mutable getOrder();
	}
}

