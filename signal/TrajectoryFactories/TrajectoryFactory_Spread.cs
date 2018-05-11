using System;
using core;

namespace signal
{
	public class TrajectoryFactory_Spread : AbstractPassiveTrajectoryFactory
	{
		private static string NAME = "Spread";
		private string _name;
		public override string Name {
			get {return _name;}
		}
		
		public override void SimulationStartNotification() {
		}
		
		public override void SimulationEndNotification() {
			MyTrajectory.add(TimeNow, Orderbook.getSpread());
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			if (Orderbook.isNonDegenerate()) {
				if (Orderbook.getSpread() >= 0.0) {
					MyTrajectory.add(TimeNow, Orderbook.getSpread());
				}
			}
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			/*
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getSpread());
				*/
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
			if (Orderbook.isNonDegenerate()) {
				if (Orderbook.getSpread() >= 0.0) {
					MyTrajectory.add(TimeNow, Orderbook.getSpread());
				}
			}
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
			if (Orderbook.isNonDegenerate()) {
				if (Orderbook.getSpread() >= 0.0) {
					MyTrajectory.add(TimeNow, Orderbook.getSpread());
				}
			}
		}
		
		public TrajectoryFactory_Spread(double timeQuantum, double historicalBias) : base(timeQuantum, historicalBias) {
			_name = NAME;
			reset();
		}
	}
}

