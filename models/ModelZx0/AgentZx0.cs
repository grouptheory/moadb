using System;
using System.Collections.Generic;
using core;
using agent;
using logger;
using orderbook;

namespace models
{
	public class AgentZx0 : AbstractAgent
	{
		protected override string BASENAME {
			get { return "AgentZx0"; }
		}
		
		private readonly static double DecideToAct_PROBABILITY = 1.0;
		private readonly static double DecideToCancelOpenOrder_PROBABILITY = 0.25;
		private readonly static double DecideToMakeOrder_PROBABILITY = 1.0;

		private readonly static double DecideToSubmitBid_PROBABILITY = 0.50;

		private readonly static double BidVolume_CONSTANT = 100.0;
		private readonly static double AskVolume_CONSTANT = 100.0;
		
		private readonly static string NetWorth_METRICNAME = "NetWorth";
		private readonly static string AgentTrades_METRICNAME = "AgentTrades";
		private readonly static string AgentOrders_METRICNAME = "AgentOrders";
		
		private readonly static string Clock_PROPERTYNAME = "Clock";
		private readonly static string PriceOffset_PROPERTYNAME = "PriceOffset";
		private readonly static string VolumeOffset_PROPERTYNAME = "VolumeOffset";
		
		private double _clock = -1.0;
		private double _priceOffset;
		private double _volumeOffset;

		private int _myTrades;
		private int _myOrders;
		
		private IOrderbookPriceEngine _pe = new OrderbookPriceEngine();

		public AgentZx0(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0)
		{
			_clock = coordinates.getCoordinate( coordinates.Space.getAxisIndex(Clock_PROPERTYNAME) );
			SetMetricValue(Clock_PROPERTYNAME, _clock);
			
			_priceOffset = coordinates.getCoordinate( coordinates.Space.getAxisIndex(PriceOffset_PROPERTYNAME) );
			SetMetricValue(PriceOffset_PROPERTYNAME, _priceOffset);
			
			_volumeOffset = coordinates.getCoordinate( coordinates.Space.getAxisIndex(VolumeOffset_PROPERTYNAME) );
			SetMetricValue(VolumeOffset_PROPERTYNAME, _volumeOffset);

			_myTrades = 0;
			SetMetricValue(AgentTrades_METRICNAME, (double)_myTrades);
			
			_myOrders = 0;
			SetMetricValue(AgentOrders_METRICNAME, (double)_myOrders);
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
			SetMetricValue(AgentTrades_METRICNAME, _myTrades);
		}
		
		private void IncrementTotalOrders() {
			_myOrders++;
			SetMetricValue(AgentOrders_METRICNAME, _myOrders);
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
		
		protected override double GetTimeToNextActionPrompt ()
		{
			if (_clock < 0) {
				// this happens in the AbstractAgent ctor, which needs to register the very first ActionPrompt
				// subsequently, _clock will have been set by using the BlauSpace coordinate
				return 0.0;
			}

			double ttnp = _clock;
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
			bool answer = (SingletonRandomGenerator.Instance.NextDouble() <= DecideToMakeOrder_PROBABILITY);
			if (answer) IncrementTotalOrders();
			return answer;
		}
		
		protected override bool DecideToSubmitBid() {
			return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToSubmitBid_PROBABILITY);
		}
		
		protected override double GetBidPrice() {
			double price = _pe.getBidPrice(this.Orderbook);
			price += _priceOffset;
			return price;
		}
		
		protected override int GetBidVolume() {
			return (int)(BidVolume_CONSTANT + _volumeOffset);
		}
		
		protected override double GetAskPrice() {
			double price = _pe.getAskPrice(this.Orderbook);
			price += _priceOffset;
			return price;
		}
		
		protected override int GetAskVolume() {
			return (int)(AskVolume_CONSTANT + _volumeOffset);
		}
	}
}

