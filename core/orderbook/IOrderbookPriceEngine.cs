using System;

namespace core
{
	public interface IOrderbookPriceEngine
	{
		double Price {
			get ;
			set ;
		}
			
		double SteadySpreadToPriceRatio {
			get ;
			set ;
		}
			
		double ModeToPriceRatio {
			get ;
			set ;
		}
			
		double MarketProb {
			get ;
			set ;
		}
			
		bool isMarket (IOrderbook_Agent ob);

		double getMarketPrice (IOrderbook_Agent ob);
		double getAskPrice (IOrderbook_Agent ob);
		double getBidPrice (IOrderbook_Agent ob);

		double getMarketPriceOffset (IOrderbook_Agent ob);
		double getAskPriceOffset (IOrderbook_Agent ob);
		double getBidPriceOffset (IOrderbook_Agent ob);
	}
}

