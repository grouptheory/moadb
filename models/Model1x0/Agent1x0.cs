using System;
using System.Collections.Generic;
using core;
using agent;
using logger;
using des;

namespace models
{
	public class Agent1x0 : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent1x0"; }
		}
		
		private readonly static int BidVolume_CONSTANT = 100;
		private readonly static int AskVolume_CONSTANT = 100;

		public readonly static string NetWorth_METRICNAME = "NetWorth";
		private readonly static string TotalTrades_METRICNAME = "TotalTrades";
		private readonly static string AgentOrders_METRICNAME = "AgentOrders";
		
		//Clock influences the frequency with which an agent acts.
		private readonly static string Clock_PROPERTYNAME = "clock";
		private double _clock;
		
		// Aggressiveness influences how quickly an agent places an order in the absence of an existing order in the market.
		private readonly static string Aggressiveness_PROPERTYNAME = "aggressiveness";
		private double _aggressiveness;
		
		// Patience influences how long an agent will allow an order to remain unexecuted in the order book.
		private readonly static string Patience_PROPERTYNAME = "patience";
		private double _patience;
		
		// Trendiness influences the direction of an order (bid / ask) based on relative changes in the volume of bids and asks in the order book (since his last action).
		private readonly static string Trendiness_PROPERTYNAME = "trendiness";
		private double _trendiness;
		
		// Optimism influences the price at which an order is made.
		private readonly static string Optimism_PROPERTYNAME = "optimism";
		private double _optimism;
		
		// Riskiness influences the size of the order.
		private readonly static string Riskiness_PROPERTYNAME = "riskiness";
		private double _riskiness;

		private double _lambda;
		private double _gamma;
			
		public Agent1x0(IBlauPoint coordinates, IAgentFactory creator, int id, double lambda, double gamma, double burnin) : base(coordinates, creator, id, burnin)
		{
			SingletonLogger.Instance().InfoLog(typeof(Agent1x0), "creating agent "+id);
			
			_clock = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Clock_PROPERTYNAME) );
			_aggressiveness = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Aggressiveness_PROPERTYNAME) );
			_patience = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Patience_PROPERTYNAME) );
			_trendiness = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Trendiness_PROPERTYNAME) );
			_optimism = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Optimism_PROPERTYNAME) );
			_riskiness = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Riskiness_PROPERTYNAME) );
			
			SetMetricValue(Clock_PROPERTYNAME, _clock);
			SetMetricValue(Aggressiveness_PROPERTYNAME, _aggressiveness);
			SetMetricValue(Patience_PROPERTYNAME, _patience);
			SetMetricValue(Trendiness_PROPERTYNAME, _trendiness);
			SetMetricValue(Optimism_PROPERTYNAME, _optimism);
			SetMetricValue(Riskiness_PROPERTYNAME, _riskiness);
			
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" have set my clock to: "+GetMetricValue(Clock_PROPERTYNAME));
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" have set my aggressiveness to: "+GetMetricValue(Aggressiveness_PROPERTYNAME));
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" have set my patience to: "+GetMetricValue(Patience_PROPERTYNAME));
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" have set my trendiness to: "+GetMetricValue(Trendiness_PROPERTYNAME));
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" have set my optimism to: "+GetMetricValue(Optimism_PROPERTYNAME));
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" have set my riskiness to: "+GetMetricValue(Riskiness_PROPERTYNAME));
			
			ResetMetrics();
			
			_haveOrder = false;
			
			_lastBidVolume = 0.0;
			_lastAskVolume = 0.0;
			
			_lambda = lambda;
			_gamma = gamma;
			
			_burninResetCompleted = false;
			_startTime = 0.0;
			
			SingletonLogger.Instance().InfoLog(typeof(Agent1x0), "done creating agent "+id);
		}
		
		private void ResetMetrics() {
			SetMetricValue(TotalTrades_METRICNAME, 0.0);
			SetMetricValue(NetWorth_METRICNAME, 0.0);
			SetMetricValue(AgentOrders_METRICNAME, 0.0);
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			AccumulateNetWorth( ValuateTransaction(filledOrder, price, volume) );
			IncrementTotalTrades();
			
				SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" EXECUTED time is "+Scheduler.GetTime()+
			                                    " filledOrder: "+filledOrder+
			                                    " Filled "+volume+" AT : "+price+
			                                    " OBprice: "+Orderbook.getPrice()+
			                                    " OBspread: "+Orderbook.getSpread()+
			                                    " My net worth is "+GetMetricValue(NetWorth_METRICNAME));
			
			_haveOrder = false;
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			AccumulateNetWorth( ValuateTransaction(partialOrder, price, volume) );
			
				SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" PARTLYEXECUTED time is "+Scheduler.GetTime()+
			                                    " filledOrder: "+partialOrder+
			                                    " Filled "+volume+" AT : "+price+
			                                    " OBprice: "+Orderbook.getPrice()+
			                                    " OBspread: "+Orderbook.getSpread() +
			                                    " My net worth is "+GetMetricValue(NetWorth_METRICNAME));
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
			
			double liquidationValue = Holdings * Orderbook.FinalPrice;
			AccumulateNetWorth(liquidationValue);
		}
		
		protected override double GetTimeToNextActionPrompt() {
			double ttnp = _clock;
			// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" GetTimeToNextActionPrompt: "+ttnp);
			return ttnp;
		}
		
		protected override bool DecideToAct() {
			
			if ((!_burninResetCompleted) && (Scheduler.GetTime () - _startTime) > this.BurninTime*3600.0) {
				ResetMetrics();
				_burninResetCompleted = true;
			}
			
			bool answer = true;
			// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I decide to act, haveOrder: "+_haveOrder+" open orders: "+OpenOrders.Count);
			return answer;
		}
		
		private bool _haveOrder;
		
		protected override bool DecideToCancelOpenOrder(IOrder openOrder) {
			bool cancel = false;
			if (_haveOrder) {
				cancel = (SingletonRandomGenerator.Instance.NextDouble() <= _patience);
				SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" CANCEL: "+cancel);
				if (cancel) _haveOrder = false;
			}
			return cancel;
		}
		
		protected override bool DecideToMakeOrder() {
			
			if ( ! Orderbook.isNonDegenerate()) {
				SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXXXXXXXX OB: " + Orderbook.ToStringLong());
				return false;
			}
			if (_haveOrder) {
				// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I cannot make a new order, I have one");
				return false;
			}
				
			bool makeorder = (SingletonRandomGenerator.Instance.NextDouble() <= _aggressiveness);
			
			// SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "I am "+this.ID+" DecideToMakeOrder: "+makeorder);
			if (makeorder) {
				IncrementTotalOrders();
				_haveOrder = true;
			}
			return makeorder;
		}
		
		private double _lastBidVolume;
		private double _lastAskVolume;
		
		protected override bool DecideToSubmitBid() {
			double bidVol = Orderbook.getBidVolume();
			double dBid = bidVol - _lastBidVolume;
			_lastBidVolume = bidVol;
			
			double askVol = Orderbook.getAskVolume();
			double dAsk = askVol - _lastAskVolume;
			_lastAskVolume = askVol;
			
			double differential = (dAsk - dBid)/(askVol + bidVol);
			
			double cutoff = 0.5 + (differential * _trendiness);
			
			double toss = SingletonRandomGenerator.Instance.NextDouble();
			
			bool makebid;
			if (toss > cutoff) {
				makebid = false;
			}
			else {
				makebid = true;
			}

			return makebid;
		}
		
		protected override double GetBidPrice() {
			double logNormal = getLogNormal();
			double price = Orderbook.getLowestAsk() - logNormal + _optimism * Orderbook.getPrice();
			double roundedPrice = Math.Round(price*100.0)/100.0;
			
				SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" BID time is "+Scheduler.GetTime()+
			                                    " myBidPrice: "+roundedPrice+
			                                    " OBprice: "+Orderbook.getPrice()+
			                                    " OBspread: "+Orderbook.getSpread() );
			return roundedPrice;
		}
		
		private static double MAX_ORDER_VOLUME_FRACTION = (1.0/3.0);
		
		protected override int GetBidVolume() {
			int answer = BidVolume_CONSTANT;
			
			double std = BidVolume_CONSTANT;
			double GaussianNumber = this._riskiness * SingletonRandomGenerator.Instance.NextGaussianPositive(0.0, std);
			
			if (GaussianNumber > Orderbook.getAskVolume() * MAX_ORDER_VOLUME_FRACTION) 
				GaussianNumber = Orderbook.getAskVolume() * MAX_ORDER_VOLUME_FRACTION;

			if (GaussianNumber < BidVolume_CONSTANT) GaussianNumber = BidVolume_CONSTANT;

			answer = (int)GaussianNumber;
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" BID Volume: "+answer);
			return answer;
		}
		
		private double getLogNormal ()
		{
			double normal = SingletonRandomGenerator.Instance.NextGaussian (0.0, 1.0);
			// offset from best opposing order
			double x = 2.0 * _lambda - 1.0;
			double a = 0.14;
			double pi = Math.PI;
			double q = (2.0 / (pi * a) + Math.Log (1.0 - x * x) / 2.0);
			double q2 = Math.Log (1.0 - x * x) / a;
			if (q * q < q2)
				throw new Exception ("qq < q2");

			double erfinvx = Math.Sqrt (Math.Sqrt (q * q - q2) - q);
			if (Math.Sqrt (q * q - q2) < q)
				throw new Exception ("Math.Sqrt(q*q - q2) < q");

			double sigma = -1.0 * Math.Sqrt (2.0) * erfinvx;

			// gamma fraction captured in twice spread
			if (Orderbook.getSpread () < 0) {
				return 0.0;
				// throw new Exception ("Orderbook.getSpread() < 0");
			}

			double mu = sigma*sigma + Math.Log( _gamma * Orderbook.getSpread() );

			double logNormal = Math.Exp(mu + sigma * normal);
			return logNormal;
		}
		
		protected override double GetAskPrice() {
			double logNormal = getLogNormal();
			double price = Orderbook.getHighestBid() + logNormal + _optimism * Orderbook.getPrice();
			double roundedPrice = Math.Round(price*100.0)/100.0;
			
				SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" ASK time is "+Scheduler.GetTime()+
			                                    " myAskPrice: "+roundedPrice+
			                                    " OBprice: "+Orderbook.getPrice()+
			                                    " OBspread: "+Orderbook.getSpread() );
			return roundedPrice;
		}
		
		protected override int GetAskVolume() {
			int answer = AskVolume_CONSTANT;
			
			double std = AskVolume_CONSTANT;
			double GaussianNumber = this._riskiness * SingletonRandomGenerator.Instance.NextGaussianPositive(0.0, std);

			if (GaussianNumber > Orderbook.getBidVolume() * MAX_ORDER_VOLUME_FRACTION) 
				GaussianNumber = Orderbook.getBidVolume() * MAX_ORDER_VOLUME_FRACTION;
			
			if (GaussianNumber < BidVolume_CONSTANT) GaussianNumber = AskVolume_CONSTANT;

			answer = (int)GaussianNumber;
			
			SingletonLogger.Instance().DebugLog(typeof(Agent1x0), "XXX I am "+this.ID+" ASK Volume: "+answer);
			return answer;
		}
	}
}

