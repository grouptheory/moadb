using System;

namespace core
{
	public interface IOrder_Mutable : IOrder
	{
		void setCancel();
		void setFilled(int vol, double p);
	}
}

