using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using core;
using des;

namespace orderbook
{
	public class OrderbookLogger : SimEntity, IOrderbookObserver
	{
		public class OrderbookBoundaries {
			private double _lowAsk;
			private double _highAsk;
			private double _lowBid;
			private double _highBid;
			
			public OrderbookBoundaries() {
				_lowAsk = -1;
				_highAsk = -1;
				_lowBid = -1;
				_highBid = -1;
			}
			
			public void process (IOrderbook_Matcher ob)
			{
				/*
				_lowAsk = ob.getLowestAsk();
				_highAsk = ob.getHighestAsk();
				_lowBid = ob.getLowestBid();
				_highBid = ob.getHighestBid();
				*/
			}
			
			public bool differentFrom (OrderbookBoundaries other)
			{
				if (this._lowAsk != other._lowAsk ||
				    this._highAsk != other._highAsk ||
				    this._lowBid != other._lowBid ||
				    this._highBid != other._highBid) {
					return true;
				}
				else return false;
			}

			public override String ToString ()
			{
				return "lowbid:"+_lowBid+"\t highbid:"+_highBid+"\t lowask:"+_lowAsk+"\t highAsk:"+_highAsk;
			}
		}

		// SimEntity
		
	    public override void Destructor() {
			// no op
		}
		
	    public override string GetName() {
			return "OrderbookLogger";
		}
	
	    public override void Recv(ISimEntity src, ISimEvent simEvent) {
			// no op
		}
		
		// OrderbookLogger
		
		private static double DUMP_INTERVAL = 1.0;
		private double _LastDumpTime;
		private int _DumpNumber;
		private bool _Virgin;
		private string _tag;

		private OrderbookBoundaries _bdy;

		// Time,Price,Direction,OrderPrice,OrderSize,
		
		public OrderbookLogger (string tag)
		{
			_tag = tag;
			_LastDumpTime = 0.0;
			_DumpNumber = 0;
			_Virgin = true;

			_bdy = new OrderbookBoundaries();
		}
		
		public void recvOrderbookNotification (IOrderbook_Matcher ob, IOrderbookEvent evt)
		{
						String line = "";
			if (evt is IOrderbookEvent_AddOrder) {
				// log an Add row
				IOrderbookEvent_AddOrder addEvt = (IOrderbookEvent_AddOrder)evt;
				IOrder order = addEvt.getOrder ();
				
				string status = "Add";
				string time = "" + String.Format ("{0:.###}", Scheduler.GetTime ());

				string price = "" + (ob.getNumBids()>0 ? String.Format ("{0:.###}", ob.getHighestBid ()) : "?")
					+ "," + (ob.getNumAsks()>0 ? String.Format ("{0:.###}", ob.getLowestAsk ()) : "?");
				string direction = "" + (order.isBid () ? "BID" : "ASK");
				string orderprice = "" + String.Format ("{0:.###}", order.getPrice ());
				string ordersize = "" + order.getVolume ();
				string owner = "\"" + order.getOwner ().ToString () + "\"";
				line = status + "," + time + "," + price + "," + direction + "," + orderprice + "," + ordersize + "," + owner;
				Console.WriteLine (line);
			} else if (evt is IOrderbookEvent_CancelOrder) {
				// log a Cancel
				IOrderbookEvent_CancelOrder cancelEvt = (IOrderbookEvent_CancelOrder)evt;
				IOrder order = cancelEvt.getOrder ();
				
				string status = "Cancel";
				string time = "" + String.Format ("{0:.###}", Scheduler.GetTime ());
				string price = "" + (ob.getNumBids()>0 ? String.Format ("{0:.###}", ob.getHighestBid ()) : "?")
					+ "," + (ob.getNumAsks()>0 ? String.Format ("{0:.###}", ob.getLowestAsk ()) : "?");string direction = "" + (order.isBid () ? "BID" : "ASK");
				string orderprice = "" + String.Format ("{0:.###}", order.getPrice ());
				string ordersize = "" + order.getVolume ();
				string owner = "\"" + order.getOwner ().ToString () + "\"";
				line = status + "," + time + "," + price + "," + direction + "," + orderprice + "," + ordersize + "," + owner;
				Console.WriteLine (line);
			} else if (evt is IOrderbookEvent_FillOrder) {
				// no op
				IOrderbookEvent_FillOrder fillEvt = (IOrderbookEvent_FillOrder)evt;
				IOrder order = fillEvt.getOrder ();
				
				string status = "Fill";
				string time = "" + String.Format ("{0:.###}", Scheduler.GetTime ());
				string price = "" + (ob.getNumBids()>0 ? String.Format ("{0:.###}", ob.getHighestBid ()) : "?")
					+ "," + (ob.getNumAsks()>0 ? String.Format ("{0:.###}", ob.getLowestAsk ()) : "?");string direction = "" + (order.isBid () ? "BID" : "ASK");
				string orderprice = "" + String.Format ("{0:.###}", order.getPrice ());
				string ordersize = "" + order.getVolume ();
				string owner = "\"" + order.getOwner ().ToString () + "\"";
				line = status + "," + time + "," + price + "," + direction + "," + orderprice + "," + ordersize + "," + owner;
				Console.WriteLine (line);
			}

			OrderbookBoundaries newbdy = new OrderbookBoundaries ();
			newbdy.process (ob);
			if (_bdy.differentFrom (newbdy)) {
				Console.WriteLine ("*** OB boundaries changed!! "+newbdy.ToString());
			}
			_bdy = newbdy;

			using (FileStream fs = new FileStream("frames/OB-"+_tag+".txt", FileMode.Append, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs)) {
				sw.WriteLine (line);
			}

			if (Scheduler.GetTime () - _LastDumpTime > DUMP_INTERVAL) {
				_LastDumpTime = Scheduler.GetTime ();

				using (FileStream fs = new FileStream("frames/BIDS-"+_tag+"-"+_DumpNumber+".dat", FileMode.Append, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(fs)) {
					
					if (_Virgin) {
						sw.WriteLine ("#===================================");
						_Virgin = true;
					}

					IDictionary<double, IList<IOrder_Mutable>> bids = ob.getBids_Mutable();
					sw.WriteLine ("# num bid: "+bids.Keys.Count);
					sw.WriteLine ("# low bid: "+(ob.getNumBids()>0 ? ""+ob.getLowestBid():"?"));
					sw.WriteLine ("# high bid: "+(ob.getNumBids()>0 ? ""+ob.getHighestBid():"?"));
					foreach (double price in bids.Keys) {
						IList<IOrder_Mutable> orders = bids[price];
						double vol = 0;
						foreach (IOrder_Mutable o in orders) {
							vol += o.getVolume();
						}
						sw.WriteLine (""+price+","+vol+"");
					}
				}

				
				using (FileStream fs = new FileStream("frames/ASKS-"+_tag+"-"+_DumpNumber+".dat", FileMode.Append, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(fs)) {
					
					if (_Virgin) {
						sw.WriteLine ("#===================================");
						_Virgin = true;
					}

					IDictionary<double, IList<IOrder_Mutable>> asks = ob.getAsks_Mutable();
					sw.WriteLine ("# num ask: "+asks.Keys.Count);
					sw.WriteLine ("# low ask: "+(ob.getNumAsks()>0 ? ""+ob.getLowestAsk():"?"));
					sw.WriteLine ("# high ask: "+(ob.getNumAsks()>0 ? ""+ob.getHighestAsk():"?"));
					foreach (double price in asks.Keys) {
						IList<IOrder_Mutable> orders = asks[price];
						double vol = 0;
						foreach (IOrder_Mutable o in orders) {
							vol += o.getVolume();
						}
						sw.WriteLine (""+price+","+vol);
					}
				}

				using (FileStream fs = new FileStream("frames/GP-"+_tag+"-"+_DumpNumber+".gp", FileMode.Append, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(fs)) {
					sw.WriteLine ("set terminal postscript eps color");
					sw.WriteLine ("set output \"EPS-"+_tag+"-"+String.Format("{0:00000000}",_DumpNumber)+".eps\"");
					sw.WriteLine ("set title \"Orderbook at time="+Scheduler.GetTime ()+" in "+_tag+"\"");
					sw.WriteLine ("set ylabel \"Orders (count)\"");
					sw.WriteLine ("set xlabel \"Price\"");
					sw.WriteLine ("binwidth=0.125");
					sw.WriteLine ("set boxwidth binwidth");
					sw.WriteLine ("bin(x,width)=width*floor(x/width) + binwidth/2.0");
					sw.WriteLine ("plot 'Asks-"+_tag+"-"+_DumpNumber+".dat' using (bin($1,binwidth)):(1.0) smooth freq with boxes t \"ASKS "+_tag+"-"+_DumpNumber+"\" fs solid 0.50, 'Bids-"+_tag+"-"+_DumpNumber+".dat' using (bin($1,binwidth)):(1.0) smooth freq with boxes t \"BIDS "+_tag+"-"+_DumpNumber+"\" fill empty");
				}

				_DumpNumber++;
			}
		}
	}
}

