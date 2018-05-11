using System;
using core;
using logger;

namespace signal
{
	public class TrajectoryFactory_TotalOrders : AbstractPassiveTrajectoryFactory
	{
		private static string NAME = "TotalOrders";
		private string _name;
		public override string Name {
			get {return _name;}
		}
		
		public override void SimulationStartNotification() {
		}
		
		public override void SimulationEndNotification() {
			MyTrajectory.add(TimeNow, Orderbook.getNumBids()+Orderbook.getNumAsks());
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getNumBids()+Orderbook.getNumAsks());
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getNumBids()+Orderbook.getNumAsks());
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getNumBids()+Orderbook.getNumAsks());
		}
		
		public TrajectoryFactory_TotalOrders(double timeQuantum, double historicalBias) : base(timeQuantum, historicalBias) {
			_name = NAME;
			reset();
		}
	}
}

