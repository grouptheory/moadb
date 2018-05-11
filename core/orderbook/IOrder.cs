using System;

namespace core
{
	public interface IOrder
	{
		int getID();
		
		bool isBid();
		bool isAsk();
		
		double getPrice();
		int getVolume();
		bool isCancelled();
		bool isFilled();
		
		double getCapital();
			
		IOrderOwner getOwner();
		
		string ToString();
		
		IOrder_Mutable clone();
	}
}

