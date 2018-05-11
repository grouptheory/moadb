using System;

namespace core
{
	public interface IOrderbookEvent_FillOrder : IOrderbookEvent
	{
		IOrder_Mutable getOrder();
		double getExecutionPrice() ;
		int getVolume();
		bool orderFilled();
	}
}

