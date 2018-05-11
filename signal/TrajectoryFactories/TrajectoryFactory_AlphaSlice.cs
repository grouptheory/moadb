using System;
using core;
using logger;

namespace signal
{
	public class TrajectoryFactory_AlphaSlice : AbstractPassiveTrajectoryFactory
	{
		// private static string NAME = "AlphaSlice";
		private string _name;
		public override string Name {
			get {return _name;}
		}

		private double _alpha;
		private bool _askBased;
		
		private double GetAlphaVolume() {
			if (_askBased) return _alpha * Orderbook.getAskVolume();
			else return _alpha * Orderbook.getBidVolume();
		}
		
		private double GetPricing() {
			if (_askBased) return Orderbook.getAskPrice(GetAlphaVolume());
			else return Orderbook.getBidPrice(GetAlphaVolume());
		}
		
		private double GetDualPricing() {
			try {
			if (_askBased) return Orderbook.getBidPrice(GetAlphaVolume());
			else return Orderbook.getAskPrice(GetAlphaVolume());
			}
			catch (Exception ex) {
				Console.WriteLine("In alpha="+_alpha);
				throw ex;
			}
		}
		
		private double GetPriceNormalizedAlphaSpread() {
			double normalizer = Orderbook.getPrice();
			
						//Console.WriteLine("GetPriceNormalizedAlphaSpread I am running alpha = "+_alpha+" and _askBased = "+_askBased);

			if (_askBased) {
				return (GetPricing() - GetDualPricing()) / normalizer;
			}
			else {
				return (GetDualPricing() - GetPricing()) / normalizer;
			}
		}
		
		public override void SimulationStartNotification() {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "SimulationStartNotification");
		}
		
		public override void SimulationEndNotification() {
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "SimulationEndNotification");
			MyTrajectory.add(TimeNow, GetPriceNormalizedAlphaSpread());
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			
			if (Orderbook.getSpread() <= 0.0) return;
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "FilledOrderNotification");
			if (Orderbook.isNonDegenerate() && MyTrajectory.ThresholdTimePassed(TimeNow)) 
				MyTrajectory.add(TimeNow, GetPriceNormalizedAlphaSpread());
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			
			if (Orderbook.getSpread() <= 0.0) return;
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "PartialFilledOrderNotification");
			if (Orderbook.isNonDegenerate() && MyTrajectory.ThresholdTimePassed(TimeNow)) 
				MyTrajectory.add(TimeNow, GetPriceNormalizedAlphaSpread());
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
			
			if (Orderbook.getSpread() <= 0.0) return;
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "NewOrderNotification");
			if (Orderbook.isNonDegenerate() && MyTrajectory.ThresholdTimePassed(TimeNow)) 
				MyTrajectory.add(TimeNow, GetPriceNormalizedAlphaSpread());
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
			
			if (Orderbook.getSpread() <= 0.0) return;
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "CancelOrderNotification");
			if (Orderbook.isNonDegenerate() && MyTrajectory.ThresholdTimePassed(TimeNow)) 
				MyTrajectory.add(TimeNow, GetPriceNormalizedAlphaSpread());
		}
		
		public TrajectoryFactory_AlphaSlice(double timeQuantum, double historicalBias,  double alpha, bool askBased) : base(timeQuantum, historicalBias) {
			
			_alpha = alpha;
			_askBased = askBased;
			
			//Console.WriteLine("CTOR I am running alpha = "+_alpha+" and _askBased = "+_askBased);
			
			if (_askBased) {
				_name = "AskSlice-"+_alpha;
			}
			else {
				_name = "BidSlice-"+_alpha;
			}
			
			reset();
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryFactory_Price), "TrajectoryFactory_AlphaSlice");
		}
	}
}

