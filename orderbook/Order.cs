using System;
using core;

namespace orderbook
{
	public class Order : IOrder_Mutable
	{
		// static ID of next order
		private static int _nextID = 0;
		
		// data members
		private int _ID;
		private IOrderOwner _owner;
		
		private bool _isbid;
		private double _price;
		private int _volume;
		private double _capital;
		private bool _cancelled;
		private bool _filled;
		
		public int getID() {
			return _ID;
		}
		
		public bool isBid() {
			return _isbid;
		}
		
		public bool isAsk(){
			return !_isbid;
		}
		
		public double getPrice() {
			return _price;
		}
		
		public int getVolume() {
			return _volume;
		}
		
		public double getCapital() {
			return _capital;
		}
		
		public bool isCancelled() {
			return _cancelled;
		}
		
		public bool isFilled() {
			return _filled;
		}
		
		public IOrderOwner getOwner() {
			return _owner;
		}
		
		public void setCancel() {		
			if (isCancelled()) {
				throw new Exception("setCancel violation: order already cancelled");
			}
			if (isFilled()) {
				throw new Exception("setCancel violation: order already filled");
			}
			
			_cancelled = true;
		}
		
		public void setFilled(int vol, double p) {			
			if (isCancelled()) {
				throw new Exception("setFilled violation: order already cancelled");
			}
			if (isFilled()) {
				throw new Exception("setFilled violation: order already filled");
			}
			
			if (isBid()) {
				if (p>getPrice()) {
					throw new Exception("setFilled on bid order: price matching violation");
				}
			}
			else {
				if (p<getPrice()) {
					throw new Exception("setFilled on ask order: price matching violation");
				}
			}
			
			if (vol > _volume) {
				throw new Exception("setFilled : volume matching violation");
			}
			
			_volume -= vol;
			
			
			if (isBid()) {
				_capital -= (vol*p);
			}
			else {
				_capital += (vol*p);
			}
			
			if (_volume <= 0) {
				_filled = true;
			}
		}
		
		// examples:
		// ASK 3u @10 [$0]
		// BID 3u @5 [$0]
		public override string ToString() {
			string str = ""+_ID;
			str += " " + (isFilled () ? "!" : (isCancelled () ? "X" : " "));
			str += " " + (isAsk() ? "ASK" : "BID");
			str += " "+_volume+"u @"+_price;
			str += " [$" + _capital +"]";
			return str;
		}
		
		public Order (bool isbid, double price, int volume, IOrderOwner owner)
		{
			_ID = _nextID;
			
			_isbid = isbid;
			_price = price;
			_volume = volume;
			_owner = owner;
			_cancelled = false;
			_filled = false;
			_capital = 0;
			
			_nextID++;
		}
		
		public IOrder_Mutable clone() {
			return new Order(this);
		}
		
		private Order (Order orig)
		{
			_ID = orig._ID;
			
			_isbid = orig._isbid;
			_price = orig._price;
			_volume = orig._volume;
			_owner = orig._owner;
			_cancelled = orig._cancelled;
			_filled = orig._filled;
			_capital = orig._capital;
		}
	}
}

