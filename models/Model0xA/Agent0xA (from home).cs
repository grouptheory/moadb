using System;
using System.Collections.Generic;
using core;
using agent;
using logger;

namespace models
{
	public class Agent0xA : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent0x3"; }
		}
		
		private readonly static double TimeToNextActionPrompt_INTERVAL = 2.0;
		private          static double DecideToAct_PROBABILITY = 0.50;
		private readonly static double DecideToCancelOpenOrder_PROBABILITY = 0.25;
		private readonly static double DecideToMakeOrder_PROBABILITY = 0.25;
		private readonly static double DecideToSubmitBid_PROBABILITY = 0.50;
		private readonly static int BidVolume_CONSTANT = 100;
		private readonly static int AskVolume_CONSTANT = 100;
		
		private readonly static string NetWorth_METRICNAME = "NetWorth";
		private readonly static string TotalTrades_METRICNAME = "TotalTrades";
		private readonly static string TotalOrders_METRICNAME = "TotalOrders";
		
		private readonly static string GainCutoff_PROPERTYNAME = "GainCutoff";
		private readonly static string LossCutoff_PROPERTYNAME = "LossCutoff";
		private readonly static string Type_PROPERTYNAME = "Type";
		
		private double _G;
		private double _L;
		private double _Tdouble;
		private int _T;
		
		private int _myTrades;
		private int _myOrders;

		public Agent0xA(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0)
		{
			_G = coordinates.getCoordinate( coordinates.Space.getAxisIndex(GainCutoff_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my aggressiveness to: "+_aggressiveness);
			SetMetricValue(GainCutoff_PROPERTYNAME, _G);
			
			_L = coordinates.getCoordinate( coordinates.Space.getAxisIndex(LossCutoff_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my optimism to: "+_optimism);
			SetMetricValue(LossCutoff_PROPERTYNAME, _L);
			
			_Tdouble = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Type_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my optimism to: "+_optimism);
			if (_Tdouble < 0.0) _T = -1;
			else _T = +1;
			SetMetricValue(Type_PROPERTYNAME, _T);
			
			_myTrades = 0;
			SetMetricValue(TotalTrades_METRICNAME, (double)_myTrades);
			
			_myOrders = 0;
			SetMetricValue(TotalOrders_METRICNAME, (double)_myOrders);
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have iniitialized TotalTrades metric to: "+GetMetricValue(TotalTrades_METRICNAME));
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
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
			_myTrades++;
			SetMetricValue(TotalTrades_METRICNAME, _myTrades);
		}
		
		private void IncrementTotalOrders() {
			_myOrders++;
			SetMetricValue(TotalOrders_METRICNAME, _myOrders);
		}
		
		private void AccumulateNetWorth(double val) {
			double netWorth = GetMetricValue(NetWorth_METRICNAME);
			SetMetricValue(NetWorth_METRICNAME, netWorth + val);
		}
		
		public override void SimulationStartNotification() {
			SetMetricValue(NetWorth_METRICNAME, 0.0);
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
		
		private double _V = 0.0;
		private double _Pstar = 0.0;
		
		protected override bool DecideToAct() {
			return true;
		}
		
		protected override bool DecideToCancelOpenOrder(IOrder openOrder) {
			return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToCancelOpenOrder_PROBABILITY);
		}
		
		protected override bool DecideToMakeOrder() {
			bool answer = (SingletonRandomGenerator.Instance.NextDouble() <= DecideToMakeOrder_PROBABILITY);
			if (answer) IncrementTotalOrders();
			return answer;
		}
		
		protected override bool DecideToSubmitBid() {
			return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToSubmitBid_PROBABILITY);
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


		protected override double GetAskPrice() {
			double logNormal = getLogNormal();
			double price = Orderbook.getHighestBid() + logNormal + _optimism * Orderbook.getPrice();
			double roundedPrice = Math.Round(price*100.0)/100.0;
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

