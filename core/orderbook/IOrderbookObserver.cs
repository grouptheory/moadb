using System;

namespace core
{
	public interface IOrderbookObserver : ISimEntity
	{
		void recvOrderbookNotification(IOrderbook_Matcher ob, IOrderbookEvent evt);
	}
}

