using System;
using System.Collections.Generic;
using core;
using agent;

namespace models
{
	public class Agent0x0 : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent0x0"; }
		}
		
		private readonly static double TimeToNextActionPrompt_INTERVAL = 2.0;
		private readonly static double DecideToAct_PROBABILITY = 0.50;
		private readonly static double DecideToCancelOpenOrder_PROBABILITY = 0.25;
		private readonly static double DecideToMakeOrder_PROBABILITY = 0.25;
		private readonly static double DecideToSubmitBid_PROBABILITY = 0.50;
		private readonly static int BidVolume_CONSTANT = 100;
		private readonly static int AskVolume_CONSTANT = 100;
		
		private readonly static string NetWorth_METRICNAME = "NetWorth";
		
		public Agent0x0(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0)
		{
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			AccumulateNetWorth( ValuateTransaction(filledOrder, price, volume) );
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
			return ttnp;
		}
		
		protected override bool DecideToAct() {
			return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToAct_PROBABILITY);
		}
		
		protected override bool DecideToCancelOpenOrder(IOrder openOrder) {
			return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToCancelOpenOrder_PROBABILITY);
		}
		
		protected override bool DecideToMakeOrder() {
			return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToMakeOrder_PROBABILITY);
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
			roundedPrice += 0.03;
			return roundedPrice;
		}
		
		protected override int GetAskVolume() {
			return AskVolume_CONSTANT;
		}
	}
}

