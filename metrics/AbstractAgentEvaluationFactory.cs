using System;
using core;
using logger;

namespace metrics
{
	public abstract class AbstractAgentEvaluationFactory : IAgentEvaluationFactory
	{
		private ISimulationParameters _sim;
		
		protected IOrderbook_Agent Orderbook {
			get { 
				if (_sim==null) return null;
				else return _sim.Orderbook; 
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
			// events received from ISimulation
			
			if (se.OrderbookEvent == null) {
				if (se is ISimulationStart) {
					SingletonLogger.Instance().DebugLog(typeof(AbstractAgentEvaluationFactory), "*** AbstractAgentEvaluationFactory got ISimulationStart");
					_sim = sim;
					reset();
					SimulationStartNotification();
				}
				
				if (se is ISimulationEnd) {
					SingletonLogger.Instance().DebugLog(typeof(AbstractAgentEvaluationFactory), "*** AbstractAgentEvaluationFactory got ISimulationEnd");
					SimulationEndNotification();
					_sim = null;
				}
			}
			else {
				SingletonLogger.Instance().DebugLog(typeof(AbstractAgentEvaluationFactory), "*** AbstractAgentEvaluationFactory got ISimulationEvent with non-null Orderbook");
				
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
		
		private IAgentEvaluation _evaluation;
		protected IAgentEvaluation MyAgentEvaluation {
			get { return _evaluation; }
		}
		
		public IAgentEvaluation create() {
			IAgentEvaluation answer = MyAgentEvaluation;
			reset();
			return answer;
		}
		
		public void reset() {
			_evaluation = new AgentEvaluation(this.Name, this);
		}
		
		protected AbstractAgentEvaluationFactory (string name)
		{
			_name = name;
			reset();
		}
		
		private string _name;
		public string Name {
			get { return _name; }
		}
		
		
		public abstract void SimulationStartNotification();
		public abstract void SimulationEndNotification();
		
		public abstract void FilledOrderNotification(IOrder filledOrder, double price, int volume);
		public abstract void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume);
		public abstract void NewOrderNotification(IOrder newOrder);
		public abstract void CancelOrderNotification(IOrder cancelledOrder);
		
		public override string ToString ()
		{
			string s = "AbstractAgentEvaluationFactory '"+Name+"' w/ "+MyAgentEvaluation;
			return s;
		}
	}
}

