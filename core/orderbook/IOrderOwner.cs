using System;

namespace core
{
	public interface IOrderOwner
	{
		void recvOrderNotification(IOrderbook_Agent ob, IOrderbookEvent evt);
	}
}

