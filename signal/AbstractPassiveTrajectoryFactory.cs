using System;
using core;
using logger;

namespace signal
{
	public abstract class AbstractPassiveTrajectoryFactory : IPassiveTrajectoryFactory
	{
		private ISimulationParameters _sim;
		
		protected IOrderbook_Matcher Orderbook {
			get {  
				if (_sim==null) return null;
				else return (IOrderbook_Matcher)_sim.Orderbook; 
			}
		}
		
		protected IPopulation Population {
			get { 
				if (_sim==null) return null;
				else return _sim.Population; 
			}
		}
		
		protected double TimeNow {
			get { 
				if (_sim==null) return -1.0;
				else return _sim.Time; 
			}
		}
		
		public void recvSimulationNotification(ISimulationParameters sim, ISimulationEvent se) {
			
			SingletonLogger.Instance().DebugLog(typeof(AbstractPassiveTrajectoryFactory), "AbstractPassiveTrajectoryFactory recvSimulationNotification");
			
			// events received from ISimulation
			
			if (se.OrderbookEvent == null) {
				if (se is ISimulationStart) {
					SingletonLogger.Instance().DebugLog(typeof(AbstractPassiveTrajectoryFactory), "AbstractPassiveTrajectoryFactory SET Sim!");
					_sim = sim;
					reset();
					SimulationStartNotification();
				}
				
				if (se is ISimulationEnd) {
					SimulationEndNotification();
					SingletonLogger.Instance().DebugLog(typeof(AbstractPassiveTrajectoryFactory), "AbstractPassiveTrajectoryFactory CLEAR Sim!");
					_sim = null;
				}
			}
			else {
				if (_sim==null) {
					// too late
					SingletonLogger.Instance().DebugLog(typeof(AbstractPassiveTrajectoryFactory), "AbstractPassiveTrajectoryFactory Too Late!");
					return;
				}
			
				if (se.OrderbookEvent is IOrderbookEvent_FillOrder) {
					IOrderbookEvent_FillOrder fillEvent = (IOrderbookEvent_FillOrder)se.OrderbookEvent;
					IOrder filledOrder = fillEvent.getOrder();
					if (fillEvent.orderFilled()) {
						FilledOrderNotification(filledOrder, fillEvent.getExecutionPrice(), fillEvent.getVolume());
					}
					else {
						PartialFilledOrderNotification(filledOrder, fillEvent.getExecutionPrice(), fillEvent.getVolume());
					}
				}
				else if (se.OrderbookEvent is IOrderbookEvent_AddOrder) {
					IOrderbookEvent_AddOrder addEvent = (IOrderbookEvent_AddOrder)se.OrderbookEvent;
					IOrder newOrder = addEvent.getOrder();
					NewOrderNotification(newOrder);
				}
				else if (se.OrderbookEvent is IOrderbookEvent_CancelOrder) {
					IOrderbookEvent_CancelOrder cancelEvent = (IOrderbookEvent_CancelOrder)se.OrderbookEvent;
					IOrder cancelledOrder = cancelEvent.getOrder();
					CancelOrderNotification(cancelledOrder);
				}
			}
		}
		
		private ITrajectory _trajectory;
		protected ITrajectory MyTrajectory {
			get { return _trajectory; }
		}

		private double _temporalGranularityThreshold;
		public double TemporalGranularityThreshold {
			get { return _temporalGranularityThreshold;}
		}
		
		private double _historicalBias;
		
		public ITrajectory create() {
			ITrajectory answer = MyTrajectory;
			return answer;
		}
		
		public void reset() {
			_trajectory = new Trajectory(this.Name, TemporalGranularityThreshold, _historicalBias, _burnin);
		}
		
		private double _burnin;
		protected double BurninTime {
			get { return _burnin; }
		}
		
		public void Initialize(ITableGenerationConfig config) {
			_burnin = config.InitialBurninHours;
			_trajectory = new Trajectory(this.Name, TemporalGranularityThreshold, _historicalBias, _burnin);
		}
		
		protected AbstractPassiveTrajectoryFactory (double timeQuantum, double historicalBias)
		{
			_temporalGranularityThreshold = timeQuantum;
			_historicalBias = historicalBias;
			_burnin = 0.0;
		}
		
		public abstract string Name {
			get;
		}
		
		public abstract void SimulationStartNotification();
		public abstract void SimulationEndNotification();
		
		public abstract void FilledOrderNotification(IOrder filledOrder, double price, int volume);
		public abstract void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume);
		public abstract void NewOrderNotification(IOrder newOrder);
		public abstract void CancelOrderNotification(IOrder cancelledOrder);
		
		public override string ToString ()
		{
			string s = "AbstractPassiveTrajectoryFactory '"+Name+"' w/ "+MyTrajectory;
			return s;
		}
	}
}

