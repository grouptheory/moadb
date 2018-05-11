using System;
using System.Collections.Generic;

namespace core
{
	public interface IOrderbook_Agent
	{
		IOrder addBid(double price, int volume, IAgent owner);
		IOrder addAsk(double price, int volume, IAgent owner);
		bool cancelOrder(IOrder order);
		
		IDictionary<double, IList<IOrder>> getBids();
		double getHighestBid();
		double getLowestBid();
		int getNumBids();
		double getBidVolume();

		IDictionary<double, IList<IOrder>> getAsks();
		double getLowestAsk();
		double getHighestAsk();
		int getNumAsks();
		double getAskVolume();
		
		bool isNonDegenerate();
		double getPrice();
		double getSpread();
		
		IOrderbook_Observable clone();
		
		string ToStringLong ();
		
		double getAskPrice(double vol);
		double getBidPrice(double vol);
		
		 double FinalPrice {
			get;
			set;
		}
	}
}

