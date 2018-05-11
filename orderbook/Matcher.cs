using System;
using System.Collections.Generic;
using des;
using core;
using logger;

namespace orderbook
{
	public class Matcher : SimEntity, IMatcher
	{
		// IMatcher
		static private ILog _logger = SingletonLogger.Instance();
		static private Type SOURCE = typeof(Matcher);
		
		// statististics
		private int _NumAddOrdersReceived;
		private int _NumFillsSent;
		private int _NumPartialFillsSent;
		
		// accounting for inbound signals
		public int NumAddOrdersReceived {
			get { return _NumAddOrdersReceived; }
		}
		
		// accounting for outbound signals
		public int NumFillsSent {
			get { return _NumFillsSent; }
		}
		
		// accounting for outbound signals
		public int NumPartialFillsSent {
			get { return _NumPartialFillsSent; }
		}
		
		// SimEntity
		
	    public override void Destructor() {
			// no op
		}
		
	    public override string GetName() {
			return "Matcher";
		}
	
	    public override void Recv(ISimEntity src, ISimEvent simEvent) {
			// no op
			// throw new Exception("Unexpected call to Matcher.Recv");
		}
		
		// The Matcher receives an IOrderbook event of type OrderbookEvent_AddOrder
		// whenever a bid or ask order is added to the Orderbook.  This event is delivered 
		// asynchronously, thereby allowing orders to be added to the OB without agents
		// needing to be written in a re-entrant manner.
		
		public void recvOrderbookNotification(IOrderbook_Matcher ob, IOrderbookEvent evt) {
			
			_NumAddOrdersReceived++;
			
			_logger.DebugLog(SOURCE, "Matcher receives "+evt);
			
			IOrder_Mutable order = null;
			if (evt is OrderbookEvent_AddOrder) {
				OrderbookEvent_AddOrder ordevt = (OrderbookEvent_AddOrder)evt;
				order = ordevt.getOrder();
				
				if (order.isFilled()) {
					return;
				}
				
				// either the order is a bid
				if (order.isBid()) {
					do {
						if (ob.getNumAsks() > 0) {
							double p = ob.getLowestAsk();
							// asks exist which can execute
							// begin execution at the lowest ask
							if (order.getPrice() >= p) {
								_logger.DebugLog(SOURCE, "Matcher attempts fill on "+order);
								fill(order, p, ob);
							}
							else {
								break;
							}
						}
						else {
							break;
						}
					}
					while (order.getVolume() > 0);
				}
				else { // or the order is an ask
					do {
						if (ob.getNumBids() > 0) {
							double p = ob.getHighestBid();
							// bids exist which can execute
							// begin execution at the higest bid
							if (order.getPrice() <= p) {
								_logger.DebugLog(SOURCE, "Matcher attempts fill on "+order);
								fill(order, p, ob);
							}
							else {
								break;
							}
						}
						else {
							break;
						}
					}
					while (order.getVolume() > 0);
				}
			}
			else {
				_logger.DebugLog(SOURCE, "Matcher attempts fill on "+order);
			}
		}
		
		private void fill(IOrder_Mutable order, double p, IOrderbook_Matcher ob) {
			
			IDictionary<double, IList<IOrder_Mutable>> asksDict = ob.getAsks_Mutable();
			IDictionary<double, IList<IOrder_Mutable>> bidsDict = ob.getBids_Mutable();
			
			IList<IOrder_Mutable> orderlist;
			if ( ! order.isBid() ) {
				orderlist = bidsDict[p];
			}
			else {
				orderlist = asksDict[p];
			}
			
			// use the dual orders at p to fill the pending order
			while (orderlist!=null && orderlist.Count>0) {
				IOrder_Mutable o2 = orderlist[0];
				
				int tradeVolume = Math.Min (o2.getVolume(), order.getVolume());
				
				
				// collide o2 against the order
				double executionPrice;
				
				// symmetrized logic
				if (order.isAsk()) {
					// an Ask arrived
					// the execution price is the Bid price (per conversation with KD)
					executionPrice = o2.getPrice();
				}
				else {
					// a Bid arrived
					// the execution price is the Ask price (per conversation with KD)
					executionPrice = o2.getPrice();
				}
					
				_logger.DebugLog(SOURCE, "Matcher pairs: (A) "+order+" and (B) "+o2+" -- tradeVol="+tradeVolume+" price="+executionPrice);
				
				o2.setFilled(tradeVolume, executionPrice);
				order.setFilled(tradeVolume, executionPrice);
			
				_logger.DebugLog(SOURCE, "Matcher post trade: (A) "+order+" and (B) "+o2);
				
				// if o2 gets filled, keep going through other orders at p
				if (o2.isFilled()) {
					ob.flushOrder(o2);
					
					if ( ! order.isBid() ) {
						if (bidsDict.ContainsKey(p)) {
							orderlist = bidsDict[p];
						}
						else {
							orderlist = null;
						}
					}
					else {
						if (asksDict.ContainsKey(p)) {
							orderlist = asksDict[p];
						}
						else {
							orderlist = null;
						}
					}
					
					// notify o2's owner of fill
					_NumFillsSent++;
					_logger.DebugLog(SOURCE, "Matcher notify o2's owner of fill");
					ob.notifyObserverSynchronous(new OrderbookEvent_FillOrder(o2, executionPrice, tradeVolume, true));
				}
				else {
					// notify o2's owener of partial fill
					_NumPartialFillsSent++;
					_logger.DebugLog(SOURCE, "Matcher notify o2's owner of partial fill");
					ob.notifyObserverSynchronous(new OrderbookEvent_FillOrder(o2, executionPrice, tradeVolume, false));
				}
				
				// if order get filled
				if (order.isFilled()) {
					ob.flushOrder(order);
					// notify order's owner of fill
					_NumFillsSent++;
					_logger.DebugLog(SOURCE, "Matcher notify o2's owner of fill");
					ob.notifyObserverSynchronous(new OrderbookEvent_FillOrder(order, executionPrice, tradeVolume, true));
					break;
				}
				else {
					// notify order's owner of partial fill
					_NumPartialFillsSent++;
					_logger.DebugLog(SOURCE, "Matcher notify o2's owner of partial fill");
					ob.notifyObserverSynchronous(new OrderbookEvent_FillOrder(order, executionPrice, tradeVolume, false));
				}
			}
		}
	
		// constructor
		public Matcher ()
		{
			reset ();
		}
		
		public void reset() {
			// initialize counters
			_NumAddOrdersReceived=0;
			_NumFillsSent=0;
			_NumPartialFillsSent=0;
		}
		
		public override string ToString ()
		{
			string s = "Matcher (TotalOrdersSeen:"+_NumAddOrdersReceived+", Fills:"+_NumFillsSent+", PartialFills:"+_NumPartialFillsSent+")";
			return s;
		}
	}
}

