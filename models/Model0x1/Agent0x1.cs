using System;
using System.Collections.Generic;
using core;
using agent;
using logger;

namespace models
{
	public class Agent0x1 : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent0x1"; }
		}
		
		private readonly static double TimeToNextActionPrompt_INTERVAL = 2.0;
		private readonly static double DecideToAct_PROBABILITY = 0.50;
		private readonly static double DecideToCancelOpenOrder_PROBABILITY = 0.25;
		private readonly static double DecideToMakeOrder_PROBABILITY = 0.25;
		private readonly static double DecideToSubmitBid_PROBABILITY = 0.50;
		private readonly static int BidVolume_CONSTANT = 100;
		private readonly static int AskVolume_CONSTANT = 100;
		
		private readonly static string NetWorth_METRICNAME = "NetWorth";
		private readonly static string TotalTrades_METRICNAME = "TotalTrades";
		private readonly static string TotalOrders_METRICNAME = "AgentOrders";
		
		private readonly static string Aggressiveness_PROPERTYNAME = "aggressiveness";
		
		private double _aggressiveness;
		
		public Agent0x1(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0)
		{
			SingletonLogger.Instance().InfoLog(typeof(Agent0x1), "creating agent "+id);
			
			_aggressiveness = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Aggressiveness_PROPERTYNAME) );
			
			SetMetricValue(Aggressiveness_PROPERTYNAME, _aggressiveness);
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" have set my aggressiveness to: "+GetMetricValue(Aggressiveness_PROPERTYNAME));
			
			SetMetricValue(TotalTrades_METRICNAME, 0.0);
			SetMetricValue(NetWorth_METRICNAME, 0.0);
			SetMetricValue(TotalOrders_METRICNAME, 0.0);
			
			validateMetrics(7);
			SingletonLogger.Instance().InfoLog(typeof(Agent0x1), "done creating agent "+id);
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			AccumulateNetWorth( ValuateTransaction(filledOrder, price, volume) );
			IncrementTotalTrades();
			validateMetrics(7);
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
			validateMetrics(7);
			return val;
		}
		
		private void IncrementTotalTrades() {
			// System.Environment.Exit(-1);
			double mytrades = GetMetricValue(TotalTrades_METRICNAME);
			mytrades++;
			SetMetricValue(TotalTrades_METRICNAME, mytrades);
			validateMetrics(7);
		}
		
		private void IncrementTotalOrders() {
			// System.Environment.Exit(-1);
			double myorders = GetMetricValue(TotalOrders_METRICNAME);
			myorders++;
			SetMetricValue(TotalOrders_METRICNAME, myorders);
			validateMetrics(7);
		}
		
		private void AccumulateNetWorth(double val) {
			double netWorth = GetMetricValue(NetWorth_METRICNAME);
			SetMetricValue(NetWorth_METRICNAME, netWorth + val);
			validateMetrics(7);
		}
		
		public override void SimulationStartNotification(IPopulation pop) {
			validateMetrics(7);
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
			validateMetrics(7);
		}
		
		protected override double GetTimeToNextActionPrompt() {
			double ttnp = TimeToNextActionPrompt_INTERVAL;
			ttnp += SingletonRandomGenerator.Instance.NextGaussian(0.0, 0.001);
			
			ttnp -= _aggressiveness;
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" TTNP: "+ttnp);
			
			return ttnp;
		}
		
		protected override bool DecideToAct() {
			validateMetrics(7);
			bool answer = (SingletonRandomGenerator.Instance.NextDouble() <= DecideToAct_PROBABILITY);
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" DecideToAct: "+answer);
			return answer;
		}
		
		protected override bool DecideToCancelOpenOrder(IOrder openOrder) {
			validateMetrics(7);
			bool answer = (SingletonRandomGenerator.Instance.NextDouble() <= DecideToCancelOpenOrder_PROBABILITY);
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" DecideToCancelOpenOrder: "+answer);
			return answer;
		}
		
		protected override bool DecideToMakeOrder() {
			validateMetrics(7);
			bool answer = (SingletonRandomGenerator.Instance.NextDouble() <= DecideToMakeOrder_PROBABILITY);
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" DecideToMakeOrder: "+answer);
			if (answer) {
				IncrementTotalOrders();
			}
			return answer;
		}
		
		protected override bool DecideToSubmitBid() {
			validateMetrics(7);
			bool answer = (SingletonRandomGenerator.Instance.NextDouble() <= DecideToSubmitBid_PROBABILITY);
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" DecideToSubmitBid: "+answer);
			return answer;
		}
		
		protected override double GetBidPrice() {
			double mean = (Orderbook.getNumBids() > 0 ? Orderbook.getHighestBid() : (Orderbook.getNumAsks() > 0 ? Orderbook.getLowestAsk() : Orderbook.getPrice() ));
			double std = Orderbook.getSpread();
			//std = 0.02;
			double price = SingletonRandomGenerator.Instance.NextGaussianPositive(mean, std);
			double roundedPrice = Math.Round(price*100.0)/100.0;
			roundedPrice -= 0.03;
			validateMetrics(7);
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" GetBidPrice: "+roundedPrice+" -- price = "+price+" ; mean = "+mean+" spread = "+Orderbook.getSpread());
			return roundedPrice;
		}
		
		protected override int GetBidVolume() {
			validateMetrics(7);
			int answer = BidVolume_CONSTANT;
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" GetBidVolume: "+answer);
			return answer;
		}
		
		protected override double GetAskPrice() {
			double mean = (Orderbook.getNumAsks() > 0 ? Orderbook.getLowestAsk() : (Orderbook.getNumBids() > 0 ? Orderbook.getHighestBid() : Orderbook.getPrice() ));
			double std = Orderbook.getSpread();
			//std = 0.02;
			double price = SingletonRandomGenerator.Instance.NextGaussianPositive(mean, std);
			double roundedPrice = Math.Round(price*100.0)/100.0;
			roundedPrice += 0.03;
			validateMetrics(7);
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" GetAskPrice: "+roundedPrice+" -- price = "+price+" ; mean = "+mean+" spread = "+Orderbook.getSpread());
			return roundedPrice;
		}
		
		protected override int GetAskVolume() {
			validateMetrics(7);
			int answer = AskVolume_CONSTANT;
			SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I am "+this.ID+" GetAskVolume: "+answer);
			return answer;
		}
	}
}

