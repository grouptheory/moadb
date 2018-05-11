using System;
using System.Collections.Generic;
using core;
using agent;
using logger;

namespace models
{
	public class Agent0x2 : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent0x2"; }
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
		private readonly static string TotalOrders_METRICNAME = "TotalOrders";
		
		private readonly static string Aggressiveness_PROPERTYNAME = "Aggressiveness";
		private readonly static string Optimism_PROPERTYNAME = "Optimism";
		
		private double _aggressiveness;
		private double _optimism;
		private int _myTrades;
		private int _myOrders;
		
		public Agent0x2(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0)
		{
			_aggressiveness = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Aggressiveness_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my aggressiveness to: "+_aggressiveness);
			SetMetricValue(Aggressiveness_PROPERTYNAME, _aggressiveness);
			
			_optimism = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Optimism_PROPERTYNAME) );
			//SingletonLogger.Instance().DebugLog(typeof(Agent0x1), "I have set my optimism to: "+_optimism);
			SetMetricValue(Optimism_PROPERTYNAME, _optimism);
			
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
		
		public override void SimulationStartNotification(IPopulation pop) {
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
			
			ttnp -= _aggressiveness;
			
			return ttnp;
		}
		
		protected override bool DecideToAct() {
			bool answer = (SingletonRandomGenerator.Instance.NextDouble() <= DecideToAct_PROBABILITY);
			return answer;
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
		
		protected override double GetBidPrice() {
			double mean = Orderbook.getHighestBid();
			double std = Orderbook.getLowestAsk() - Orderbook.getPrice();
			std = 0.02;
			double price = SingletonRandomGenerator.Instance.NextGaussianPositive(mean, std);
			double roundedPrice = Math.Round(price*100.0)/100.0;
			
			roundedPrice = (roundedPrice + _optimism);
			
			roundedPrice -= 0.03;
			return roundedPrice;
		}
		
		protected override int GetBidVolume() {
			return BidVolume_CONSTANT;
		}
		
		protected override double GetAskPrice() {
			double mean = Orderbook.getLowestAsk();
			double std = Orderbook.getPrice() - Orderbook.getHighestBid();
			std = 0.02;
			double price = SingletonRandomGenerator.Instance.NextGaussianPositive(mean, std);
			double roundedPrice = Math.Round(price*100.0)/100.0;
			
			roundedPrice = (roundedPrice + _optimism);
			
			roundedPrice += 0.03;
			return roundedPrice;
		}
		
		protected override int GetAskVolume() {
			return AskVolume_CONSTANT;
		}
	}
}

