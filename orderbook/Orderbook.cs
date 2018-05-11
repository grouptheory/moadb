using System;
using System.Collections.Generic;
using des;
using core;
using logger;

namespace orderbook
{
	public class Orderbook : SimEntity, IOrderbook_Observable, IOrderbook_Matcher, IOrderbook_Agent
	{
		static private ILog _logger = SingletonLogger.Instance();
		static private Type SOURCE = typeof(Orderbook);
			
		private C5.TreeDictionary<double, IList<IOrder_Mutable>> _bids;
		private C5.TreeDictionary<double, IList<IOrder_Mutable>> _asks;
		private IDictionary<double, IList<IOrder_Mutable>> _bidsDict;
		private IDictionary<double, IList<IOrder_Mutable>> _asksDict;
		private int _numBids;
		private int _numAsks;
		
		// SimEntity
		
		public class NotifyObserverEvent : ISimEvent
		{
			IOrderbookEvent _evt;
			IOrderbook_Matcher _ob;
			public NotifyObserverEvent(IOrderbook_Matcher ob, IOrderbookEvent evt) {
				_ob = ob;
				_evt = evt;
			}
    		public void Entering(ISimEntity locale) {
				if (locale is  IOrderbookObserver) {
					IOrderbookObserver obs = (IOrderbookObserver)locale;
					obs.recvOrderbookNotification(_ob,_evt);
				}
			}
		}
		
		class NotifyMatcherEvent : ISimEvent {
			IOrderbookEvent _evt;
			IOrderbook_Matcher _ob;
			public NotifyMatcherEvent(IOrderbook_Matcher ob, IOrderbookEvent evt) {
				_ob = ob;
				_evt = evt;
			}
    		public void Entering(ISimEntity locale) {
				if (locale is  IMatcher) {
					IMatcher m = (IMatcher)locale;
					m.recvOrderbookNotification(_ob,_evt);
				}
			}
		}
		
	    public override void Destructor() {
			// no op
		}
		
	    public override string GetName() {
			return "Orderbook";
		}
	
	    public override void Recv(ISimEntity src, ISimEvent simEvent) {
			// no op
			// throw new Exception("Unexpected call to Orderbook.Recv");
		}
	
		// IOrderbook_Observable
		
		public void setMatcher(IMatcher matcher) {
			_matcher = matcher;
		}
		
		public void addObserver(IOrderbookObserver observer) {
			_obsList.Add(observer);
		}
		
		public void removeObserver(IOrderbookObserver observer) {
			_obsList.Remove(observer);
		}

		public IMatcher getMatcher() {
			return _matcher;
		}
		
		public IList<IOrderbookObserver> getObserver() {
			return _obsList;
		}
		
		public bool notifyObserverSynchronous (IOrderbookEvent evt)
		{
			
			_logger.DebugLog (SOURCE, "OB gets notifyObserverSynchronous");
			
			bool result = false;
			foreach (IOrderbookObserver obs in _obsList) {
				_logger.DebugLog (SOURCE, "OB delegates to Sim " + obs.GetType ());
				obs.recvOrderbookNotification (this, evt);
				result = true;
			}
			return result;
		}
		
		private IMatcher _matcher;
		private IList<IOrderbookObserver> _obsList;
		
		// IOrderbook_Matcher
		
		public IDictionary<double, IList<IOrder_Mutable>> getBids_Mutable() {
			return _bidsDict;
		}
		
		public IDictionary<double, IList<IOrder_Mutable>> getAsks_Mutable() {
			return _asksDict;
		}
		
		public IDictionary<double, IList<IOrder>> getBids() {
			return (IDictionary<double, IList<IOrder>>)_bidsDict;
		}
		
		public IDictionary<double, IList<IOrder>> getAsks() {
			return (IDictionary<double, IList<IOrder>>)_asksDict;
		}
		
		public double getHighestBid() {
			return (double)_bids.FindMax().Key;
		}
		
		public double getLowestBid() {
			return (double)_bids.FindMin().Key;
		}
		
		public double getHighestAsk() {
			return (double)_asks.FindMax().Key;
		}

		public double getLowestAsk() {
			return (double)_asks.FindMin().Key;
		}
		
		private static double PUSH_OUT_FACTOR = 2.0;
		
		public double getAskPrice(double vol) {
			if (_asks.Count>0) {
				double vtot = 0.0;
				bool virgin = true;
				double p=-1.0;
				//Console.WriteLine ("********************************************");
				do {
					if (virgin) {
						p = (double)_asks.FindMin().Key;
						virgin = false;
					}
					else {
						C5.KeyValuePair<double, IList<IOrder_Mutable>> kvp;
						if (_asks.TrySuccessor(p, out kvp)) {
							double newp = kvp.Key;
							if (newp==p) {
								Console.WriteLine ("newp = p = "+p);
								Console.WriteLine ("Orderbook = "+this.ToStringLong());
								Environment.Exit(-1);
							}
							
							p = newp;
						}
						else {
							Console.WriteLine ("ORDERBOOK ASYMMETRY CONDITION in getAskPrice, searching for AskPrice to capture BidVolume = "+vol);
							Console.WriteLine ("Orderbook = "+this.ToStringLong());
							Environment.Exit(-1);
							
							return p + PUSH_OUT_FACTOR * (p - getLowestAsk());
						}
					}
					
					IList<IOrder_Mutable> orders = _asks[p];
					foreach (IOrder_Mutable o in orders) {
						
						if (!o.isFilled() && !o.isCancelled()) {
							vtot += o.getVolume();
								if (vtot >= vol) {
								return p;
							}
						}
					}
					//Console.WriteLine ("* xxx p="+p);
				}
				while (true);
			}
			return -1.0;
		}
		
		public double getBidPrice(double vol) {
			if (_bids.Count>0) {
				double vtot = 0.0;
				bool virgin = true;
				double p=-1.0;
				//Console.WriteLine ("********************************************");
				do {
					if (virgin) {
						p = (double)_bids.FindMax().Key;
						virgin = false;
					}
					else {
						C5.KeyValuePair<double, IList<IOrder_Mutable>> kvp;
						if (_bids.TryPredecessor(p, out kvp)) {
							double newp = kvp.Key;
							if (newp==p) {
								Console.WriteLine ("newp = p = "+p);
								Console.WriteLine ("Orderbook = "+this.ToStringLong());
								Environment.Exit(-1);
							}
							else {
								//Console.WriteLine ("newp = "+newp+" != p = "+p);
							}
							
							p = newp;
						}
						else {
							Console.WriteLine ("ORDERBOOK IMBALANCE CONDITION in getBidPrice, searching for BidPrice to capture AskVolume = "+vol);
							Console.WriteLine ("Orderbook = "+this.ToStringLong());
							throw new Exception("frobby");
							
							// return p - PUSH_OUT_FACTOR * (getHighestBid() - p);
						}
					}
					
					IList<IOrder_Mutable> orders = _bids[p];
					foreach (IOrder_Mutable o in orders) {
						vtot += o.getVolume();
						if (vtot >= vol) {
							return p;
						}
					}
					//Console.WriteLine ("* yyy p="+p);
				}
				while (true);
			}
			return -1.0;
		}
		
		public bool isNonDegenerate() {
			return true; // (_asks.Count>0 && _bids.Count>0);
		}
		
		public double getSpread ()
		{
			double spread = 0.0;

			/*
			if (isNonDegenerate ()) {
				_price = (getHighestBid () + getLowestAsk ()) / 2.0;
				spread = getLowestAsk () - getHighestBid ();
			}
			*/

			if (_numBids > 0 && _numAsks > 0) {
				_price = (getHighestBid () + getLowestAsk ()) / 2.0;
				spread = getLowestAsk () - getHighestBid ();
			} else if (_numBids > 0 && _numAsks == 0) {
				_price = getHighestBid ();
				spread = 0.0;
			} else if (_numBids == 0 && _numAsks > 0) {
				_price = getLowestAsk ();
				spread = 0.0;
			} else {
				spread = 0.0;
			}
		
			// spread can be negative when market order arrives
			// if (spread < 0) throw new Exception("bad spread: "+this.ToStringLong ());
			return spread;
		}

		private static double INITIAL_BASE_PRICE = 10.0;

		double _price = INITIAL_BASE_PRICE;
		public double getPrice() {
			/*
			if (isNonDegenerate())
				_price = (getHighestBid () + getLowestAsk ())/2.0;
			*/

			if (_numBids > 0 && _numAsks > 0) {
				_price = (getHighestBid () + getLowestAsk ()) / 2.0;
			} else if (_numBids > 0 && _numAsks == 0) {
				_price = getHighestBid ();
			} else if (_numBids == 0 && _numAsks > 0) {
				_price = getLowestAsk ();
			} else {
			}

			return _price;
		}
		
		public double getBidVolume() {
			double vol = 0.0;
			foreach (IList<IOrder_Mutable> listo in _bidsDict.Values) {
				foreach (IOrder o in listo) {
					if (!o.isFilled() && !o.isCancelled()) {
						vol += o.getVolume();
					}
				}
			}
			return vol;
		}
		
		public double getAskVolume() {
			double vol = 0.0;
			foreach (IList<IOrder_Mutable> listo in _asksDict.Values) {
				foreach (IOrder o in listo) {
					if (!o.isFilled() && !o.isCancelled()) {
						vol += o.getVolume();
					}
				}
			}
			return vol;
		}
		
		
		// IOrderbook_Agent
		
		private double _finalPrice;
		public double FinalPrice {
			get { return _finalPrice; }
			set { _finalPrice = value; }
		}
		
		public int getNumBids() {
			return _numBids;
		}
		
		public IOrder addBid(double price, int volume, IAgent owner) {
			IList<IOrder_Mutable> val;
			if ( ! _bidsDict.TryGetValue(price, out val)) {
				val = new List<IOrder_Mutable>();
				_bidsDict.Add(price, val);
				_bids.Add(price, val);
			}
			IOrder_Mutable order = new Order(true, price, volume, owner);
			val.Add(order);
			_numBids++;
			
			_logger.DebugLog(SOURCE, "Orderbook addBid "+order);
			
			if (_matcher != null) {
				this.Send(_matcher, new NotifyMatcherEvent(this, new OrderbookEvent_AddOrder(order)), 0.0);
			}
			
			foreach (IOrderbookObserver obs in _obsList) {
				this.Send(obs, new NotifyObserverEvent(this, new OrderbookEvent_AddOrder(order)), 0.0);
			}

			/*
			if (isNonDegenerate())
				_price = (getHighestBid () + getLowestAsk ())/2.0;
			*/
			getPrice(); // updates _price

			return order;
		}
		
		public int getNumAsks() {
			return _numAsks;
		}
		
		public IOrder addAsk(double price, int volume, IAgent owner) {
			IList<IOrder_Mutable> val;
			if ( ! _asksDict.TryGetValue(price, out val)) {
				val = new List<IOrder_Mutable>();
				_asksDict.Add(price, val);
				_asks.Add(price, val);
			}
			IOrder_Mutable order = new Order(false, price, volume, owner);
			val.Add(order);
			_numAsks++;
			
			_logger.DebugLog(SOURCE, "Orderbook addAsk "+order);
			
			if (_matcher != null) {
				this.Send(_matcher, new NotifyMatcherEvent(this, new OrderbookEvent_AddOrder(order)), 0.0);
			}
			
			foreach (IOrderbookObserver obs in _obsList) {
				this.Send(obs, new NotifyObserverEvent(this, new OrderbookEvent_AddOrder(order)), 0.0);
			}

			/*
			if (isNonDegenerate())
				_price = (getHighestBid () + getLowestAsk ())/2.0;
			*/
			getPrice(); // updates _price

			return order;
		}
		
		public bool cancelOrder(IOrder order) {
			IList<IOrder_Mutable> val;
			bool result = false;
			
			_logger.DebugLog(SOURCE, "Orderbook cancelOrder "+order);
			
			if (order.isBid()) {
				if ( ! _bidsDict.TryGetValue(order.getPrice(), out val)) {
					return false;
				}
				else {
					((IOrder_Mutable)order).setCancel();
					result = val.Remove((IOrder_Mutable)order);
					_numBids--;
				}
			}
			else {
				if ( ! _asksDict.TryGetValue(order.getPrice(), out val)) {
					return false;
				}
				else {
					((IOrder_Mutable)order).setCancel();
					result = val.Remove((IOrder_Mutable)order);
					_numAsks--;
				}
			}
			
			if (val.Count==0) {
				if (order.isBid()) {
					_bidsDict.Remove(order.getPrice());
					_bids.Remove(order.getPrice());
				}
				else {
					_asksDict.Remove(order.getPrice());
					_asks.Remove(order.getPrice());
				}
			}
			
			foreach (IOrderbookObserver obs in _obsList) {
				this.Send(obs, new NotifyObserverEvent(this, new OrderbookEvent_CancelOrder((IOrder_Mutable)order)), 0.0);
			}
			
			return result;
		}
		
		public bool flushOrder(IOrder_Mutable order) {		
			
			_logger.DebugLog(SOURCE, "Orderbook flushOrder "+order);
			
			IList<IOrder_Mutable> val;
			if (order.isBid()) {
				_bidsDict.TryGetValue(order.getPrice(), out val);
			}
			else {
				_asksDict.TryGetValue(order.getPrice(), out val);
			}
			bool result = val.Remove(order);
			
			if (order.isBid()) {
				_numBids--;
			}
			else {
				_numAsks--;
			}
			
			if (val.Count==0) {
				if (order.isBid()) {
					_bidsDict.Remove(order.getPrice());
					_bids.Remove(order.getPrice());
				}
				else {
					_asksDict.Remove(order.getPrice());
					_asks.Remove(order.getPrice());
				}
			}
			return result;
		}
		
		// Constructors
		
		public Orderbook ()
		{
			_bidsDict = new Dictionary<double, IList<IOrder_Mutable>>();
			_bids = new C5.TreeDictionary<double,IList<IOrder_Mutable>>();
			
			_asksDict = new Dictionary<double, IList<IOrder_Mutable>>();
			_asks = new C5.TreeDictionary<double,IList<IOrder_Mutable>>();
			
			_numBids = _numAsks = 0;
			
			_matcher = new Matcher();
			_obsList = new List<IOrderbookObserver>();
		}
		
		public IOrderbook_Observable clone() {
			return new Orderbook(this);
		}
		
		private Orderbook(Orderbook orig) {
			_bidsDict = new Dictionary<double, IList<IOrder_Mutable>>();
			_bids = new C5.TreeDictionary<double, IList<IOrder_Mutable>>();
			foreach (double p in orig._bidsDict.Keys) {
				IList<IOrder_Mutable> v = orig._bidsDict[p];
				IList<IOrder_Mutable> v2 = new List<IOrder_Mutable>();
				foreach (IOrder_Mutable o in v) {
					v2.Add(o.clone());
				}
				_bidsDict.Add(p, v2);
				_bids.Add (p, v2);
			}
			
			_asksDict = new Dictionary<double, IList<IOrder_Mutable>>();
			_asks = new C5.TreeDictionary<double, IList<IOrder_Mutable>>();
			foreach (double p in orig._asksDict.Keys) {
				IList<IOrder_Mutable> v = orig._asksDict[p];
				IList<IOrder_Mutable> v2 = new List<IOrder_Mutable>();
				foreach (IOrder o in v) {
					v2.Add(o.clone());
				}
				_asksDict.Add(p, v2);
				_asks.Add (p, v2);
			}
			
			_numBids = orig._numBids;
			_numAsks = orig._numAsks;
			
			_matcher = orig._matcher;
			_matcher.reset();
			_obsList = new List<IOrderbookObserver>();
			foreach (IOrderbookObserver obs in orig._obsList) {
				_obsList.Add(obs);
			}
		}
		
		public override string ToString ()
		{
			string s = "Orderbook asks:"+getNumAsks()+", bids:"+getNumBids();
			return s;
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			s+= "Total Ask Volume = "+this.getAskVolume()+"\n";
			s+= "Total Bid Volume = "+this.getBidVolume()+"\n";
			s += "===== Asks\n";
			
			foreach (double p in _asks.Keys) {
				IList<IOrder_Mutable> list = _asksDict[p];
				s += ("$"+p+" ==> ");
				foreach (IOrder_Mutable ask in list) {
					s += (""+ask+"; ");
				}
				s += "\n";
			}
			s += "===== Bids\n";
			foreach (double p in _bids.Keys) {
				IList<IOrder_Mutable> list = _bidsDict[p];
				s += ("$"+p+" ==> ");
				foreach (IOrder_Mutable bid in list) {
					s += (""+bid+"; ");
				}
				s += "\n";
			}
			s += ""+_matcher.ToString();
			
			return s;
		}
		
	}
}

