using System;
using System.Collections.Generic;
using core;
using agent;
using logger;
using des;

namespace models
{
	public class Agent0xA : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent0xA"; }
		}

		private readonly static double TimeToNextActionPrompt_INTERVAL = 2.0;
		private readonly static double DecideToCancelOpenOrder_PROBABILITY = 0.25;
		private readonly static int BidVolume_CONSTANT = 100;
		private readonly static int AskVolume_CONSTANT = 100;
		
		private readonly static string NetWorth_METRICNAME = "NetWorth";
		private readonly static string TotalTrades_METRICNAME = "TotalTrades";
		private readonly static string AgentOrders_METRICNAME = "AgentOrders";
		
		private readonly static string GainCutoff_PROPERTYNAME = "GainCutoff";
		private readonly static string LossCutoff_PROPERTYNAME = "LossCutoff";
		private readonly static string Clock_PROPERTYNAME = "Clock";
		private readonly static string Type_PROPERTYNAME = "AgentType";
		
		private double _G;
		private double _L;
		private double _TypeDouble;
		private int _Type;
		private double _clock;

		public Agent0xA(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0)
		{
			_G = coordinates.getCoordinate( coordinates.Space.getAxisIndex(GainCutoff_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my aggressiveness to: "+_aggressiveness);
			SetMetricValue(GainCutoff_PROPERTYNAME, _G);
			if (_G < 0.0 ) throw new Exception("Gain cutoff must be positive!");

			_L = coordinates.getCoordinate( coordinates.Space.getAxisIndex(LossCutoff_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my optimism to: "+_optimism);
			SetMetricValue(LossCutoff_PROPERTYNAME, _L);
			if (_L < 0.0 ) throw new Exception("Loss cutoff must be positive!");
			
			_TypeDouble = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Type_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my optimism to: "+_optimism);

			if (_TypeDouble < 0.0) _Type = -1; // play short
			else _Type = +1; // play long
			SetMetricValue(Type_PROPERTYNAME, _Type);

			_clock = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Clock_PROPERTYNAME) );
			SetMetricValue(Clock_PROPERTYNAME, _clock);
			
			ResetMetrics();
			
			_burninResetCompleted = false;
			_startTime = 0.0;

			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have iniitialized TotalTrades metric to: "+GetMetricValue(TotalTrades_METRICNAME));
		}

		private void ResetMetrics() {
			SetMetricValue(TotalTrades_METRICNAME, 0.0);
			SetMetricValue(NetWorth_METRICNAME, 0.0);
			SetMetricValue(AgentOrders_METRICNAME, 0.0);
		}

		public override void FilledOrderNotification (IOrder filledOrder, double price, int volume)
		{
			if (filledOrder.isAsk()) {
				_holdings -= volume;
			} 
			else {
				_holdings += volume;
			}
			_Pstar = price;
			_Tstar = Scheduler.GetTime();

			// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" I filled at "+Scheduler.GetTime() + " price "+price+" HOLD:"+_holdings);
			// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX OB "+Scheduler.GetTime() + ", "+this.Orderbook.getNumAsks()+", "+this.Orderbook.getNumBids());

			AccumulateNetWorth( ValuateTransaction(filledOrder, price, volume) );
			IncrementTotalTrades();
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			AccumulateNetWorth( ValuateTransaction(partialOrder, price, volume) );
		}
		
		private double ValuateTransaction(IOrder order, double price, int volume) {
			double val = 0.0;
			if (order.isAsk()) {
				val += (price * volume);
			}
			else {
				val -= (price * volume);
			}
			return val;
		}
		
		private void IncrementTotalTrades() {
			// System.Environment.Exit(-1);
			double mytrades = GetMetricValue(TotalTrades_METRICNAME);
			mytrades++;
			SetMetricValue(TotalTrades_METRICNAME, mytrades);
		}
		
		private void IncrementTotalOrders() {
			// System.Environment.Exit(-1);
			double myorders = GetMetricValue(AgentOrders_METRICNAME);
			myorders++;
			SetMetricValue(AgentOrders_METRICNAME, myorders);
		}

		private void AccumulateNetWorth(double val) {
			double netWorth = GetMetricValue(NetWorth_METRICNAME);
			SetMetricValue(NetWorth_METRICNAME, netWorth + val);
		}
		
		private double _startTime;
		private bool _burninResetCompleted;

		public override void SimulationStartNotification(IPopulation pop) {
			_startTime = Scheduler.GetTime();
		}
		
		public override void SimulationEndNotification() {
			CancelAllOpenOrders();
			// don't use instantaneous price
			// double liquidationValue = Holdings * Orderbook.getPrice();
			// use uniform "final price" set in Simulation post-run
			
			// DEBUGGING XXX
			double liquidationValue = Holdings * Orderbook.FinalPrice;
			//double liquidationValue = 0.0;
			
			AccumulateNetWorth(liquidationValue);
		}
		
		protected override double GetTimeToNextActionPrompt() {
			double ttnp = TimeToNextActionPrompt_INTERVAL;
			ttnp += SingletonRandomGenerator.Instance.NextGaussian(0.0, 0.001);
			return ttnp;
		}
	
		// The volume I hold (<0 means short, >0 long)
		private double _holdings = 0.0;

		// Price of last fill
		private double _Pstar = 0.0;
		
		// Time of last fill
		private double _Tstar = 0.0;

		private bool _virgin = true;

		protected override bool DecideToAct ()
		{
			//SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX DecideToAct: I am "+this.ID+" time is "+Scheduler.GetTime() + " I have "+OpenOrdersCount ()+ " open orders");

			if ((!_burninResetCompleted) && (Scheduler.GetTime () - _startTime) > this.BurninTime * 3600.0) {
				ResetMetrics ();
				_burninResetCompleted = true;
			}

			if (_virgin) {
				_Pstar = this.Orderbook.getPrice ();
				_Tstar = Scheduler.GetTime ();
				if (_Pstar > 0.0) {
					_virgin = false;
				}
				else {
					return false;
				}
			}

			return true;
		}
		
		protected override bool DecideToCancelOpenOrder (IOrder openOrder)
		{
			//SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX DecideToCancelOpenOrder: I am "+this.ID+" time is "+Scheduler.GetTime() + " I have "+OpenOrdersCount ()+ " open orders");

			bool cancel = (SingletonRandomGenerator.Instance.NextDouble () <= DecideToCancelOpenOrder_PROBABILITY);
			if (cancel) {
				// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" time is "+Scheduler.GetTime() + " CANCELLING my order -- I have "+OpenOrdersCount ()+ " open orders right now");
				// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX OB "+Scheduler.GetTime() + ", "+this.Orderbook.getNumAsks()+", "+this.Orderbook.getNumBids());
			}
			else {
				// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" time is "+Scheduler.GetTime() + " NOT CANCELLING my order -- I have "+OpenOrdersCount ()+ " open orders right now");
			}
			return cancel;
		}

		private bool _bid = false;
		private bool _market = false;

		private static readonly double GPERC = 0.90;
		private static readonly double LPERC = 0.90;
		private static readonly double TPERC = 0.90;
		private static readonly double EPSILON = 0.001;

		private void decideMarketOrLimit (double p)
		{
			double toss = SingletonRandomGenerator.Instance.NextUniform(0.0, 1.0);
			if (toss < p) {
				_market =true;
			}
			else {
				_market = false;
			}
		}

		protected override bool DecideToMakeOrder ()
		{
			//SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX DecideToMakeOrder: I am "+this.ID+" time is "+Scheduler.GetTime() + " I have "+OpenOrdersCount ()+ " open orders");

			if (OpenOrdersCount () > 0) {
				// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" time is "+Scheduler.GetTime() + " NOT making an order because I have "+OpenOrdersCount ()+ " open orders");
				return false;
			}

			bool choice = false;
			_market = false;

			double currentPrice = Orderbook.getPrice ();
			if (_Type > 0) { // long player
				double gainAbs = currentPrice - _Pstar;
				double relGain = gainAbs / _Pstar;
				if (relGain > 0) {
					double p = relGain<=EPSILON ? 0.0 : Math.Exp (_G * Math.Log (GPERC) / relGain); 
					double toss = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
					if (toss < p) {
						if (_holdings > 0) { // long player with holdings... time to lock in profit by sell
							_bid = false;
							decideMarketOrLimit (p);
						} else { // time to get in on the runup by buying
							_bid = true;
							decideMarketOrLimit (p);
						}
						choice = true;
					}
				} else {
					double relLoss = -1.0 * relGain;
					double p = relLoss<=EPSILON ? 0.0 : Math.Exp (_L * Math.Log (LPERC) / relLoss); 
					double toss = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
					if (toss < p) {
						if (_holdings > 0) { // long player with holdings... time to cut losses
							_bid = false;
							decideMarketOrLimit (p);
						} else { // time to get in on the low priced stock
							_bid = true;
							decideMarketOrLimit (p);
						}
						choice = true;
					}
				}
			} else { // short player
				double gainAbs = _Pstar - currentPrice;
				double relGain = gainAbs / _Pstar;
				if (relGain > 0) {
					double p = relGain<=EPSILON ? 0.0 : Math.Exp (_G * Math.Log (GPERC) / relGain); 
					double toss = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
					if (toss < p) {
						if (_holdings < 0) { // short player with holdings.. time to lock in profit by buying
							_bid = true;
							decideMarketOrLimit (p);
						} else { // time to get in on the price fall by selling
							_bid = false;
							decideMarketOrLimit (p);
						}
						choice = true;
					}
				} else {
					double relLoss = -1.0 * relGain;
					double p = relLoss<=EPSILON ? 0.0 : Math.Exp (_L * Math.Log (LPERC) / relLoss); 
 					double toss = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
					if (toss < p) {
						if (_holdings < 0) { // short player with holdings... time to cut losses by buying
							_bid = true;
							decideMarketOrLimit (p);
						} else { // time to get in on the over priced stock by selling
							_bid = false;
							decideMarketOrLimit (p);
						}
						choice = true;
					}
				}
			}

			if (choice) {
				// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" time is "+Scheduler.GetTime() + " MAKING an order due to PRICE flux / market = "+this._market);
			}

			// if we have not made an order based on price flux, we still need to 
			// consider orders being placed due to time since last fill
			if (choice == false) {
				double DT = Scheduler.GetTime () - _Tstar;
				double p = DT<=EPSILON ? 0.0 : Math.Exp (_clock * Math.Log (TPERC) / DT); 
				double toss = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
				if (toss < p) {
					// its been too long... 

					if (_Type > 0) { // long player
						if (_holdings > 0) { // have holdings
							_bid = false; // sell
							decideMarketOrLimit (p);
						} else {
							_bid = true; // buy
							decideMarketOrLimit (p);
						}
					} else { // short player
						if (_holdings < 0) { // have holdings
							_bid = true; // buy
							decideMarketOrLimit (p);
						} else {
							_bid = false; // sell
							decideMarketOrLimit (p);
						}
					}
					choice = true;
				}

				if (choice) {
					// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" time is "+Scheduler.GetTime() + " MAKING an order due to TIME flux / market = "+this._market);
				}
			}

			if (choice) {
				IncrementTotalOrders ();
			}

			return choice;
		}
		
		protected override bool DecideToSubmitBid() {
			return _bid;
		}

		private double logNormalP(double x, double m, double s) {
			return Math.Exp( -Math.Pow((Math.Log(x)-m),2.0)/(2.0*Math.Pow(s,2.0)) ) / (x*s*Math.Sqrt(2.0*Math.PI));
		}

		private const int SPREAD_ARRAYSIZE = 100;
		private const int UNIFORM_ARRAYSIZE = 10;
		private const double WMAX = 0.10;

		private static double[,] _memo = new double[SPREAD_ARRAYSIZE,UNIFORM_ARRAYSIZE];
		private static bool _memoInitialized = false;

		private static void initialize ()
		{
			if (_memoInitialized) return;

			for (int i=0; i<SPREAD_ARRAYSIZE; i++) {
				for (int j=0; j<UNIFORM_ARRAYSIZE; j++) {
					_memo[i,j] = Double.NaN;
				}
			}

			_memoInitialized = true;
		}

		private static double lookup(double W, double uniformToss) 
		{
			if (!_memoInitialized) initialize();

			int Windex = (int)((double)SPREAD_ARRAYSIZE * W / WMAX);
			if (Windex >= SPREAD_ARRAYSIZE) Windex = SPREAD_ARRAYSIZE-1;
			
			int TossIndex = (int)(uniformToss * (double)UNIFORM_ARRAYSIZE);
			if (TossIndex >= UNIFORM_ARRAYSIZE) TossIndex = UNIFORM_ARRAYSIZE-1;

			return _memo[Windex,TossIndex];
		}

		private static void store(double W, double uniformToss, double logNormalToss) {
			if (!_memoInitialized) initialize();

			int Windex = (int)((double)SPREAD_ARRAYSIZE * W / WMAX);
			if (Windex >= SPREAD_ARRAYSIZE) Windex = SPREAD_ARRAYSIZE-1;

			int TossIndex = (int)(uniformToss * (double)UNIFORM_ARRAYSIZE);
			if (TossIndex >= UNIFORM_ARRAYSIZE) TossIndex = UNIFORM_ARRAYSIZE-1;

			_memo[Windex,TossIndex] = logNormalToss;
		}

		private double getLogNormalToss (double W, double uniformToss)
		{
			double logNormalToss = lookup(W, uniformToss);

			if ( ! Double.IsNaN(logNormalToss)) {
				return logNormalToss;
			}

			// Assuming B=1.25 the mean offset is 1.25 * spread/2 
			// Assuming A=0.1 so 10% of the orders close the gap
			double m = -0.1339371612 + Math.Log (0.6250000000 * W);
			double s = 0.1636686660;

			double accum = 0.0;
			double stepsize = 0.0001;
			for (double x = stepsize; ; x+=stepsize) {
				accum += stepsize * logNormalP(x,m,s);
				if (accum > uniformToss) {
					logNormalToss = x;
					break;
				}
			}

			store(W, uniformToss, logNormalToss) ;

			return logNormalToss;
		}

		private double getOffset ()
		{
			double W = Orderbook.getSpread ();
			double uniformToss = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
			double logNormalToss = getLogNormalToss (W, uniformToss) ;
			return logNormalToss;
		}

		
		protected override double GetBidPrice ()
		{
			if (_market) {
				return Orderbook.getLowestAsk();
			}

			// limit order
			double off = getOffset();
			double basePrice = Orderbook.getPrice(); // Orderbook.getLowestAsk(); //Orderbook.getHighestBid()
			double price = basePrice - off;
			double roundedPrice = Math.Round(price*100.0)/100.0;

			/*
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" time is "+Scheduler.GetTime() +" myBidPrice: "+roundedPrice+
			                                    " OBprice: "+Orderbook.getPrice()+
			                                    " OBspread: "+Orderbook.getSpread() + " I have "+OpenOrdersCount ()+ " open orders");
			*/

			return roundedPrice;
		}


		protected override double GetAskPrice() {
			if (_market) {
				return Orderbook.getHighestBid();
			}

			// limit order
			double off = getOffset();
			double basePrice = Orderbook.getPrice(); // Orderbook.getHighestBid(); //Orderbook.getLowestAsk()
			double price = basePrice + off;
			double roundedPrice = Math.Round(price*100.0)/100.0;

			/*
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" time is "+Scheduler.GetTime() +" myAskPrice: "+roundedPrice+
			                                    " OBprice: "+Orderbook.getPrice()+
			                                    " OBspread: "+Orderbook.getSpread() + " I have "+OpenOrdersCount ()+ " open orders");
			*/

			return roundedPrice;
		}

		
		protected override int GetBidVolume() {
			return BidVolume_CONSTANT;
		}

		protected override int GetAskVolume() {
			return AskVolume_CONSTANT;
		}
	}
}

