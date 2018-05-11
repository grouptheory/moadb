using System;
using core;
using logger;

namespace signal
{
	public class TrajectoryFactory_Debug : AbstractPassiveTrajectoryFactory, ITrajectoryFactory_Ignore
	{
		private static string NAME = "Debug";
		private string _name;
		public override string Name {
			get {return _name;}
		}
		
		public override void SimulationStartNotification() {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "SimulationStartNotification");
		}
		
		public override void SimulationEndNotification() {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "SimulationEndNotification");
		
			Console.WriteLine("DEBUG"+Orderbook.ToStringLong ());
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
		}
		
		public TrajectoryFactory_Debug(double timeQuantum, double historicalBias) : base(timeQuantum, historicalBias) {
			_name = NAME;
			reset();
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "TrajectoryFactory_Debug");
		}
	}
}

