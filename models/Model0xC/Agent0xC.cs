using System;
using System.Collections.Generic;
using core;
using agent;
using orderbook;

namespace models
{
	public class Agent0xC : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent0xC"; }
		}
		
		private readonly static double TimeToNextActionPrompt_INTERVAL = 2.0;
		private readonly static double DecideToAct_PROBABILITY = 0.50;
		private readonly static double DecideToCancelOpenOrder_PROBABILITY = 0.25;
		private readonly static double DecideToMakeOrder_PROBABILITY = 0.25;
		private readonly static double DecideToSubmitBid_PROBABILITY = 0.50;
		private readonly static int BidVolume_CONSTANT = 100;
		private readonly static int AskVolume_CONSTANT = 100;
		
		private readonly static string NetWorth_METRICNAME = "NetWorth";

		private IOrderbookPriceEngine _pe = new OrderbookPriceEngine();

		public Agent0xC(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0)
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
			double price = _pe.getBidPrice(Orderbook);
			double roundedPrice = price; // Math.Round(price*100.0)/100.0;
			return roundedPrice;
		}
		
		protected override int GetBidVolume() {
			return BidVolume_CONSTANT;
		}
		
		protected override double GetAskPrice() {
			double price = _pe.getAskPrice(Orderbook);
			double roundedPrice = price; // Math.Round(price*100.0)/100.0;
			return roundedPrice;
		}
		
		protected override int GetAskVolume() {
			return AskVolume_CONSTANT;
		}
	}
}

