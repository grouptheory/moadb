using System;
using core;
using logger;

namespace signal
{
	public class TrajectoryFactory_TotalTrades : AbstractPassiveTrajectoryFactory
	{
		private static string NAME = "TotalTrades";
		private string _name;
		public override string Name {
			get {return _name;}
		}
		
		public override void SimulationStartNotification() {
		}
		
		public override void SimulationEndNotification() {
			MyTrajectory.add(TimeNow, (Orderbook.getMatcher().NumFillsSent+Orderbook.getMatcher().NumPartialFillsSent)/2.0 );
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			MyTrajectory.add(TimeNow, (Orderbook.getMatcher().NumFillsSent+Orderbook.getMatcher().NumPartialFillsSent)/2.0 );
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			MyTrajectory.add(TimeNow, (Orderbook.getMatcher().NumFillsSent+Orderbook.getMatcher().NumPartialFillsSent)/2.0 );
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
		}
		
		public TrajectoryFactory_TotalTrades(double timeQuantum, double historicalBias) : base(timeQuantum, historicalBias) {
			_name = NAME;
			reset();
		}
	}
}

