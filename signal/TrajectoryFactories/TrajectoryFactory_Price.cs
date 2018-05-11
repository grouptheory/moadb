using System;
using core;
using logger;

namespace signal
{
	public class TrajectoryFactory_Price : AbstractPassiveTrajectoryFactory
	{
		private static string NAME = "Price";
		private string _name;
		public override string Name {
			get {return _name;}
		}
		
		public override void SimulationStartNotification() {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "SimulationStartNotification");
		}
		
		public override void SimulationEndNotification() { 

			// Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX "+this+";  time now is: "+TimeNow);

			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "SimulationEndNotification");
			MyTrajectory.add(TimeNow, Orderbook.getPrice());
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "FilledOrderNotification");
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getPrice());
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "PartialFilledOrderNotification");
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getPrice());
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "NewOrderNotification");
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getPrice());
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "CancelOrderNotification");
			if (Orderbook.isNonDegenerate()) 
				MyTrajectory.add(TimeNow, Orderbook.getPrice());
		}
		
		public TrajectoryFactory_Price(double timeQuantum, double historicalBias) : base(timeQuantum, historicalBias) {
			_name = NAME;
			reset();
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "TrajectoryFactory_Price");
		}
	}
}

