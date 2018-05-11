using System;

namespace core
{
	public interface IMatcher : IOrderbookObserver, ISimEntity
	{
		int NumAddOrdersReceived {
			get;
		}
		int NumFillsSent {
			get;
		}
		int NumPartialFillsSent {
			get;
		}
		
		void reset();
		
	}
}

