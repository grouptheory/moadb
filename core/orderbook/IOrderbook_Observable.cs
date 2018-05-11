using System;

namespace core
{
	public interface IOrderbook_Observable : IOrderbook_Matcher
	{
		void setMatcher(IMatcher matcher);

		void addObserver(IOrderbookObserver observer);
		void removeObserver(IOrderbookObserver observer);
	}
}

