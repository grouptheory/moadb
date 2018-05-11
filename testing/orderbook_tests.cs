using System;
using System.IO;
using NUnit.Framework;
using logger;
using core;
using orderbook;
using des;

namespace testing
{
	[TestFixture()]
	public class orderbook_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(orderbook_tests), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		[Test()]
		public void SimpleMatcherTest()
		{
			Console.WriteLine("MatcherTest");
			Orderbook ob = new Orderbook ();
			Matcher m = new Matcher();
			ob.setMatcher(m);
			
			int ct = 0;
			// bids
			IOrder o1 = ob.addBid(10.0, 3, null);
			Scheduler.Instance().Run ();
			ct++; Assert.AreEqual(m.NumAddOrdersReceived, ct, "order "+ct);
			Assert.AreEqual(m.NumFillsSent, 0, "fills "+0);
			Assert.AreEqual(m.NumPartialFillsSent, 0, "partial fills "+0);
			Console.WriteLine("o1 = "+o1);
			
			IOrder o2 = ob.addBid(12.0, 3, null);
			Scheduler.Instance().Run ();
			ct++; Assert.AreEqual(m.NumAddOrdersReceived, ct, "order "+ct);
			Assert.AreEqual(m.NumFillsSent, 0, "fills "+0);
			Assert.AreEqual(m.NumPartialFillsSent, 0, "partial fills "+0);
			Console.WriteLine("o2 = "+o2);
			
			IOrder o3 = ob.addBid(14.0, 3, null);
			Scheduler.Instance().Run ();
			ct++; Assert.AreEqual(m.NumAddOrdersReceived, ct, "order "+ct);
			Assert.AreEqual(m.NumFillsSent, 0, "fills "+0);
			Assert.AreEqual(m.NumPartialFillsSent, 0, "partial fills "+0);
			Console.WriteLine("o3 = "+o3);
			
			// asks
			IOrder o4 = ob.addAsk(10.0, 9, null);
			Scheduler.Instance().Run ();
			// should get matched to bid @14,12,10
			// generating 4 fills (for ask and 3 bids) and 2 partial fills for the ask.
			ct++; Assert.AreEqual(m.NumAddOrdersReceived, ct, "order "+ct);
			Assert.AreEqual(m.NumFillsSent, 4, "fills "+4);
			Assert.AreEqual(m.NumPartialFillsSent, 2, "partial fills "+2);
			Console.WriteLine("o4 = "+o4);
			
			IOrder o5 = ob.addAsk(5.0, 1, null);
			Scheduler.Instance().Run ();
			// should generate no new matching
			ct++; Assert.AreEqual(m.NumAddOrdersReceived, ct, "order "+ct);
			Assert.AreEqual(m.NumFillsSent, 4, "fills "+4);
			Assert.AreEqual(m.NumPartialFillsSent, 2, "partial fills "+2);
			Console.WriteLine("o5 = "+o5);
			
			IOrder o6 = ob.addAsk(5.0, 1, null);
			Scheduler.Instance().Run ();
			// should generate no new matching
			ct++; Assert.AreEqual(m.NumAddOrdersReceived, ct, "order "+ct);
			Assert.AreEqual(m.NumFillsSent, 4, "fills "+4);
			Assert.AreEqual(m.NumPartialFillsSent, 2, "partial fills "+2);
			Console.WriteLine("o6 = "+o6);
		}
		
		
		[Test()]
		public void Orderbook_AddOrdersTest()
		{
			Console.WriteLine("Orderbook_AddOrdersTest");
			Orderbook ob = new Orderbook();
			Matcher m = new Matcher();
			ob.setMatcher(m);
			
			// no bids or asks
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 0);
			
			// place ask 1
			IOrder o1 = ob.addAsk(10.0, 3, null);
			Assert.AreEqual(ob.getNumAsks(), 1);
			Assert.AreEqual(ob.getNumBids(), 0);
			
			Assert.AreEqual(o1.isAsk(), true);
			Assert.AreEqual(o1.getPrice(), 10.0);
			Assert.AreEqual(o1.getVolume(), 3);
			Assert.AreEqual(o1.isCancelled(), false);
			Assert.AreEqual(o1.isFilled(), false);
			Assert.AreEqual(o1.getCapital(), 0.0);
			Assert.AreEqual(o1.getOwner(), null);
			
			// place bid 2
			IOrder o2 = ob.addBid(5.0, 3, null);
			Assert.AreEqual(ob.getNumAsks(), 1);
			Assert.AreEqual(ob.getNumBids(), 1);
			
			Assert.AreEqual(o2.isAsk(), false);
			Assert.AreEqual(o2.getPrice(), 5.0);
			Assert.AreEqual(o2.getVolume(), 3);
			Assert.AreEqual(o2.isCancelled(), false);
			Assert.AreEqual(o2.isFilled(), false);
			Assert.AreEqual(o2.getCapital(), 0.0);
			Assert.AreEqual(o2.getOwner(), null);
			
			Assert.AreEqual(ob.getHighestBid(), 5.0);
			Assert.AreEqual(ob.getLowestAsk(), 10.0);
			
			// place bid 3
			IOrder o3 = ob.addBid(10.0, 3, null);
			Assert.AreEqual(ob.getNumAsks(), 1);
			Assert.AreEqual(ob.getNumBids(), 2);
			
			Assert.AreEqual(o3.isAsk(), false);
			Assert.AreEqual(o3.getPrice(), 10.0);
			Assert.AreEqual(o3.getVolume(), 3);
			Assert.AreEqual(o3.isCancelled(), false);
			Assert.AreEqual(o3.isFilled(), false);
			Assert.AreEqual(o3.getCapital(), 0.0);
			Assert.AreEqual(o3.getOwner(), null);
			
			Assert.AreEqual(ob.getHighestBid(), 10.0);
			Assert.AreEqual(ob.getLowestAsk(), 10.0);
			
			// execution occurs -- o3 hits o1, leaving just the bid o2
			Scheduler.Instance().Run ();
			Assert.AreEqual(m.NumAddOrdersReceived, 3, "orders = 3");
			Assert.AreEqual(m.NumFillsSent, 2, "fills = 2");
			Assert.AreEqual(m.NumPartialFillsSent, 0, "partial fills = 0");
			
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 1);
			
			Assert.AreEqual(o3.isFilled(), true);
			Assert.AreEqual(o3.getCapital(), -30.0);
			Assert.AreEqual(o1.isFilled(), true);
			Assert.AreEqual(o1.getCapital(), 30.0);
			
			IOrder o4 = ob.addAsk(5.0, 1, null);
			IOrder o5 = ob.addAsk(5.0, 1, null);
			IOrder o6 = ob.addAsk(5.0, 1, null);
			
			Assert.AreEqual(o4.isAsk(), true);
			Assert.AreEqual(o4.getPrice(), 5.0);
			Assert.AreEqual(o4.getVolume(), 1);
			Assert.AreEqual(o4.isCancelled(), false);
			Assert.AreEqual(o4.isFilled(), false);
			Assert.AreEqual(o4.getCapital(), 0.0);
			Assert.AreEqual(o4.getOwner(), null);
			
			Assert.AreEqual(o5.isAsk(), true);
			Assert.AreEqual(o5.getPrice(), 5.0);
			Assert.AreEqual(o5.getVolume(), 1);
			Assert.AreEqual(o5.isCancelled(), false);
			Assert.AreEqual(o5.isFilled(), false);
			Assert.AreEqual(o5.getCapital(), 0.0);
			Assert.AreEqual(o5.getOwner(), null);
		
			Assert.AreEqual(o6.isAsk(), true);
			Assert.AreEqual(o6.getPrice(), 5.0);
			Assert.AreEqual(o6.getVolume(), 1);
			Assert.AreEqual(o6.isCancelled(), false);
			Assert.AreEqual(o6.isFilled(), false);
			Assert.AreEqual(o6.getCapital(), 0.0);
			Assert.AreEqual(o6.getOwner(), null);
			
			// execution occurs: 4 5 and 6 to hit o2
			Scheduler.Instance().Run ();
						
			Assert.AreEqual(o4.isFilled(), true);
			Assert.AreEqual(o4.getCapital(), 5.0);
			Assert.AreEqual(o6.isFilled(), true);
			Assert.AreEqual(o6.getCapital(), 5.0);
			Assert.AreEqual(o5.isFilled(), true);
			Assert.AreEqual(o5.getCapital(), 5.0);
			
			Assert.AreEqual(o2.isFilled(), true);
			Assert.AreEqual(o2.getCapital(), -15.0);
			
			Assert.AreEqual(m.NumAddOrdersReceived, 6, "orders = 6");
			Assert.AreEqual(m.NumFillsSent, 6, "fills = 6");
			Assert.AreEqual(m.NumPartialFillsSent, 2, "partial fills = 2");
			
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 0);
			//Assert.AreEqual(1, 0);
		}
		
		
		[Test()]
		public void Orderbook_CancelOrdersTest ()
		{
			Console.WriteLine("Orderbook_CancelOrdersTest");
			Orderbook ob = new Orderbook();
			Matcher m = new Matcher();
			ob.setMatcher(m);
			
			// no bids or asks
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 0);
			
			// place ask 1
			IOrder o1 = ob.addAsk(10.0, 3, null);
			// place bid 2
			IOrder o2 = ob.addBid(5.0, 3, null);
			
			// no bids or asks
			Assert.AreEqual(ob.getNumAsks(), 1);
			Assert.AreEqual(ob.getNumBids(), 1);
			
			bool cancelResult = ob.cancelOrder(o1);
			Assert.AreEqual(cancelResult, true);
			
			// no asks, 1 bid
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 1);
			
			Scheduler.Instance().Run ();
			
			// place bid 3
			IOrder o3 = ob.addBid(10.0, 5, null);
			
			// no asks, 2 bid
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 2);
			
			Scheduler.Instance().Run ();
			
			// execution is attempted -- but nothing can execute 
			// since o2 cannot match o3
			Scheduler.Instance().Run ();
			
			Assert.AreEqual(o1.isFilled(), false);
			Assert.AreEqual(o1.isCancelled(), true);
			Assert.AreEqual(o1.getCapital(), 0.0);
			
			Assert.AreEqual(o2.isFilled(), false);
			Assert.AreEqual(o2.isCancelled(), false);
			Assert.AreEqual(o2.getCapital(), 0.0);
			
			Assert.AreEqual(o3.isFilled(), false);
			Assert.AreEqual(o3.isCancelled(), false);
			Assert.AreEqual(o3.getCapital(), 0.0);
			
			Assert.AreEqual(m.NumAddOrdersReceived, 3, "orders = 3");
			Assert.AreEqual(m.NumFillsSent, 0, "fills = 0");
			Assert.AreEqual(m.NumPartialFillsSent, 0, "partial fills = 0");
			
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 2);
		}
		
		
		[Test()]
		public void Orderbook_ComplexMatcherTest ()
		{
			Console.WriteLine("Orderbook_ComplexMatcherTest");
			Orderbook ob = new Orderbook();
			Matcher m = new Matcher();
			ob.setMatcher(m);
			
			IOrder o2 = ob.addBid(5.0, 3, null);
			
			// let o2 fail to execute
			Scheduler.Instance().Run ();
			
			IOrder o3 = ob.addBid(10.0, 5, null);
			
			// low asks will hit an existing high bid
			IOrder o4 = ob.addAsk(5.0, 1, null);
			Assert.AreEqual(ob.getNumAsks(), 1);
			Assert.AreEqual(ob.getNumBids(), 2);
			
			IOrder o5 = ob.addAsk(5.0, 1, null);
			Assert.AreEqual(ob.getNumAsks(), 2);
			Assert.AreEqual(ob.getNumBids(), 2);
			
			IOrder o6 = ob.addAsk(5.0, 1, null);
			Assert.AreEqual(ob.getNumAsks(), 3);
			Assert.AreEqual(ob.getNumBids(), 2);
			
			// execution occurs: 4 5 and 6 partially hit 3 out of 5 units of o3 at the ask price (5)
			Scheduler.Instance().Run ();
						
			Assert.AreEqual(m.NumAddOrdersReceived, 5, "orders = 5");
			Assert.AreEqual(m.NumFillsSent, 3, "fills = 3");
			Assert.AreEqual(m.NumPartialFillsSent, 3, "partial fills = 3");
			
			Assert.AreEqual(o4.isFilled(), true);
			Assert.AreEqual(o4.getCapital(), 5.0);
			Assert.AreEqual(o6.isFilled(), true);
			Assert.AreEqual(o6.getCapital(), 5.0);
			Assert.AreEqual(o5.isFilled(), true);
			Assert.AreEqual(o5.getCapital(), 5.0);
			Assert.AreEqual(o3.isFilled(), false);
			Assert.AreEqual(o3.getCapital(), -15.0);
			Assert.AreEqual(o2.isFilled(), false);
			Assert.AreEqual(o2.getCapital(), 0.0);
			
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 2);
			
			IOrder o7 = ob.addAsk(20.0, 1, null);
			IOrder o8 = ob.addAsk(20.0, 2, null);
			IOrder o9 = ob.addAsk(20.0, 3, null);
			
			// load up the new ask orders
			Scheduler.Instance().Run ();
			
			Assert.AreEqual(m.NumAddOrdersReceived, 8, "orders = 8");
			Assert.AreEqual(m.NumFillsSent, 3, "fills = 3");
			Assert.AreEqual(m.NumPartialFillsSent, 3, "partial fills = 3");
			
			// high bid will hit existing low asks
			IOrder o10 = ob.addBid(100.0, 7, null);
			
			// execution occurs: 7 8 and 9 partially hit 10 out of 6 units of o10 at the ask price (20)
			Scheduler.Instance().Run ();
			
			Assert.AreEqual(o7.isFilled(), true);
			Assert.AreEqual(o7.getCapital(), 20.0);
			Assert.AreEqual(o8.isFilled(), true);
			Assert.AreEqual(o8.getCapital(), 40.0);
			Assert.AreEqual(o9.isFilled(), true);
			Assert.AreEqual(o9.getCapital(), 60.0);
			Assert.AreEqual(o10.isFilled(), false);
			Assert.AreEqual(o10.getCapital(), -120.0);
			
			Assert.AreEqual(m.NumAddOrdersReceived, 9, "orders = 9");
			Assert.AreEqual(m.NumFillsSent, 6, "fills = 6");
			Assert.AreEqual(m.NumPartialFillsSent, 6, "partial fills = 6");
			
			Assert.AreEqual(ob.getNumAsks(), 0);
			Assert.AreEqual(ob.getNumBids(), 3);
		}

		
		[Test()]
		public void Orderbook_PriceEngineTest ()
		{
			Console.WriteLine ("Orderbook_PriceEngineTest");
			Orderbook ob = new Orderbook ();
			Matcher m = new Matcher ();
			ob.setMatcher (m);

			double MAXTIME = 10000;

			ob.addBid (12.2, 100, null);
			ob.addAsk (12.3, 100, null);

			IOrderbookPriceEngine pe = new OrderbookPriceEngine ();
			try
			{
				System.IO.TextWriter writeFile = new StreamWriter("Orderbook_PriceEngineTest.txt");

				for (int i=0; i<MAXTIME; i++) {
					if (pe.isMarket(ob)) {
						writeFile.WriteLine("?, ?, "+pe.getMarketPriceOffset(ob));
					}
					else {
						writeFile.WriteLine(""+pe.getAskPriceOffset(ob)+", "+pe.getBidPriceOffset(ob)+", ?");
					}
				}
				writeFile.Flush();
				writeFile.Close();
				writeFile = null;
			}
			catch (IOException)
			{
				Assert.AreEqual(0, 1);
			}

			// execution occurs: 4 5 and 6 partially hit 3 out of 5 units of o3 at the ask price (5)
			Scheduler.Instance().Run ();
		}

		
		[Test()]
		public void Orderbook_PriceEngineTest_Live ()
		{
			Console.WriteLine ("Orderbook_PriceEngineTest_Live");
			Orderbook ob = new Orderbook ();
			Matcher m = new Matcher ();
			ob.setMatcher (m);
			
			double MAXTIME = 10000;
			
			ob.addBid (12.2, 100, null);
			ob.addAsk (12.3, 100, null);
			
			IOrderbookPriceEngine pe = new OrderbookPriceEngine ();
			try
			{
				System.IO.TextWriter writeFile = new StreamWriter("Orderbook_PriceEngineTest_Live.txt");
				
				for (int i=0; i<MAXTIME; i++) {
					if (pe.isMarket(ob)) {
						double pp=pe.getMarketPrice(ob);
						bool isbid=SingletonRandomGenerator.Instance.NextBoolean();
						if (isbid) ob.addBid(pp, 100, null);
						else ob.addAsk(pp, 100, null);
						writeFile.WriteLine("?, ?, "+pp);
					}
					else {
						double ppask=pe.getAskPrice(ob);
						ob.addAsk(ppask, 100, null);

						double ppbid=pe.getBidPrice(ob);
						ob.addBid(ppbid, 100, null);

						writeFile.WriteLine(""+ppask+", "+ppbid+", ?");
					}
				}
				writeFile.Flush();
				writeFile.Close();
				writeFile = null;
			}
			catch (IOException)
			{
				Assert.AreEqual(0, 1);
			}
			
			// execution occurs: 4 5 and 6 partially hit 3 out of 5 units of o3 at the ask price (5)
			Scheduler.Instance().Run ();
		}
	}
}

