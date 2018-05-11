using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using core;
using des;
using logger;

namespace agent
{
	/// <summary>
    /// AbstractAgent class
    /// </summary>
	public abstract class AbstractAgent : SimEntity, IAgent
	{
		public abstract void SimulationStartNotification(IPopulation pop);
		public abstract void SimulationEndNotification();
		public abstract void FilledOrderNotification(IOrder filledOrder, double price, int volume);
		public abstract void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume);
		
		protected abstract double GetTimeToNextActionPrompt();
		protected abstract bool DecideToAct();
		protected abstract bool DecideToCancelOpenOrder(IOrder openOrder);
		protected abstract bool DecideToMakeOrder();
		protected abstract bool DecideToSubmitBid();
		protected abstract double GetBidPrice();
		protected abstract int GetBidVolume();
		protected abstract double GetAskPrice();
		protected abstract int GetAskVolume();
		
		// an action prompt periodically hits each agent
		public class ActionPrompt : ISimEvent {
			public ActionPrompt() {}
			public void Entering(ISimEntity locale) {}
		}
		
		// the next ID to be used (static data member)
		private static int _nextID = 0;
		
		// the ID of this Agent
		private readonly int _id;
		public int ID {
			get { return _id; }
		}
		
		// the base name of the Agent, which is suffixed by ID
		protected abstract string BASENAME {
			get;
		}
		
		// the factory instance which made the Agent,
		// only this factory has the ability to clone the agent.
		private IAgentFactory _creator;
		public IAgentFactory Creator {
			get { return _creator; }
		}
		
		// the BlauSpace coordinates of this Agent
		private IBlauPoint _coordinates;
		public IBlauPoint Coordinates {
			get { return _coordinates; }
		}
		
		// the burn in time to reach steady state behavior in the ecosystem in seconds
		private double _burnin;
		protected double BurninTime {
			get { return _burnin; }
		}
		
		// the simulation ecosystem of which this Agent is part
		private ISimulationParameters _sim;
		
		// the orderbook which this Agent participates in
		protected IOrderbook_Agent Orderbook {
			get { 
				if (_sim==null) return null;
				else return _sim.Orderbook; 
			}
		}
		
		// the pending orders for this Agent
		private List<IOrder> _orders;
		protected ReadOnlyCollection<IOrder> OpenOrders {
			get { return _orders.AsReadOnly(); }
		}
		
		// basic stats: number of bids
		private readonly static string NumBids_METRICNAME = "NumBids";
		public int NumBids {
			get { return (int)GetMetricValue(NumBids_METRICNAME); }
		}
		
		// basic stats: number of bids
		private readonly static string NumAsks_METRICNAME = "NumAsks";
		public int NumAsks {
			get { return (int)GetMetricValue(NumAsks_METRICNAME); }
		}
		
		// basic stats: shares held
		private readonly static string Holdings_METRICNAME = "Holdings";
		protected int Holdings {
			get { return (int)GetMetricValue(Holdings_METRICNAME); }
		}
		
		
		// constructor
		protected AbstractAgent(IBlauPoint coordinates, IAgentFactory creator, int id, double burnin) {
			_coordinates = coordinates;
			_creator = creator;
			_burnin = burnin;
			
			// implicit assignment, allocates the ID dynamically
			if (id < 0) {
				// dynamic assignment
				_id = _nextID;
				_nextID++;
			}
			else {
				// explicit assignment (used in cloning, for example)
				_id = id;
			}
			
			// the ecosystem, initially null
			_sim = null;
			
			// the metrics associated with this Agent
			_metrics = new Dictionary<string, double>();
			
			// the list of open orders
			_orders = new List<IOrder>();
			
			// basic stats that every Agent maintains
			SetMetricValue(NumBids_METRICNAME, 0.0);
			SetMetricValue(NumAsks_METRICNAME, 0.0);
			SetMetricValue(Holdings_METRICNAME, 0.0);
			
			// boot message
			this.Send (this, new ActionPrompt(), GetTimeToNextActionPrompt());
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" Constructed @ "+Scheduler.GetTime());
		}
		
		// method to clone this agent
		public IAgent clone() {
			// ask the factory that created this Agent to clone it
			return _creator.cloneAgent(this);
		}
		
		// cancel all pending orders`
		protected void CancelAllOpenOrders() {
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" CancelAll @ "+Scheduler.GetTime());
			foreach (IOrder order in OpenOrders) {
				Orderbook.cancelOrder(order);
			}
			
			SetMetricValue(NumBids_METRICNAME, 0.0);
			SetMetricValue(NumAsks_METRICNAME, 0.0);
			_orders.Clear();
		}

		protected int OpenOrdersCount ()
		{
			return _orders.Count;
		}

		// add an order
		protected void AddToOpenOrderList(IOrder order) {
			if (_orders.Contains(order)) {
				throw new Exception("AbstractAgent "+GetName()+" is attempting to add a duplicate open order");
			}
			if (order.isAsk ()) {
				SetMetricValue(NumAsks_METRICNAME, 1+GetMetricValue(NumAsks_METRICNAME));
			}
			else {
				SetMetricValue(NumBids_METRICNAME, 1+GetMetricValue(NumBids_METRICNAME));
			}
			
			_orders.Add(order);
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), ""+GetName()+" Add "+order+" @ "+Scheduler.GetTime());
		}
		
		// remove an order
		protected void RemoveFromOpenOrderList(IOrder order) {
			if ( ! _orders.Contains(order)) {
				throw new Exception("AbstractAgent "+GetName()+"is attempting to remove an unknown open order: "+order);
			}
			
			if (order.isAsk ()) {
				SetMetricValue(NumAsks_METRICNAME, GetMetricValue(NumAsks_METRICNAME) - 1);
			}
			else {
				SetMetricValue(NumBids_METRICNAME, GetMetricValue(NumBids_METRICNAME) - 1);
			}
			
			_orders.Remove(order);
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" Remove "+order+" @ "+Scheduler.GetTime());
		}
		
		// handle events from simulation
		public void recvSimulationNotification(ISimulationParameters sim, ISimulationEvent se) {
			
			if (se.OrderbookEvent == null) {
				if (se is ISimulationStart) {
					SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" Start @ "+Scheduler.GetTime());
					_sim = sim;
					SimulationStartNotification(sim.Population);
				}
				
				if (se is ISimulationEnd) {
					SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" End @ "+Scheduler.GetTime());
					SimulationEndNotification();
					//CancelAllOpenOrders();
					_sim = null;
				}
			}
		}
		
		// accumulate into the holdings
		private void AccumulateHoldings(int val) {
			int h = (int)GetMetricValue(Holdings_METRICNAME);
			SetMetricValue(Holdings_METRICNAME, h + val);
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" AccumulateHoldings "+val+" @ "+Scheduler.GetTime()+" new holdings: "+GetMetricValue(Holdings_METRICNAME));
		}
		
		// receive a signal from the Matcher (via the Simulation, via the Population) that an order has been filled
		public void recvOrderNotification(IOrderbook_Agent ob, IOrderbookEvent evt) {
			// Fill order events
			if (evt is IOrderbookEvent_FillOrder) {
				IOrderbookEvent_FillOrder fillEvent = (IOrderbookEvent_FillOrder)evt;
				IOrder filledOrder = fillEvent.getOrder();
				
				SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" Fill "+filledOrder+" @ "+Scheduler.GetTime());
				
				if (filledOrder.isBid()) {
					AccumulateHoldings( +1*fillEvent.getVolume() );
				}
				else {
					AccumulateHoldings( -1*fillEvent.getVolume() );
				}
				
				if (fillEvent.orderFilled()) {
					RemoveFromOpenOrderList(filledOrder);
					FilledOrderNotification(filledOrder, fillEvent.getExecutionPrice(), fillEvent.getVolume());
				}
				else {
					PartialFilledOrderNotification(filledOrder, fillEvent.getExecutionPrice(), fillEvent.getVolume());
				}
			}
			// Add and Cancel events are not forwarded to agents by the Population class
		}
		
		// receive a signal from the Simulation (via the Population)
	    public override void Recv(ISimEntity src, ISimEvent simEvent) {
			if (Orderbook == null) {
				return;
			}

			if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+"recv ISimEvent "+simEvent+" @ "+Scheduler.GetTime());
			
			// events received from the DES
			if (simEvent is ActionPrompt) {
				if  (DecideToAct()) {
					
					if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+"acts @ "+Scheduler.GetTime()+" I have "+_orders.Count+" orders open");
					
					EvaluateAllOpenOrders();
					if (DecideToMakeOrder()) {
						
						if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+"makes order @ "+Scheduler.GetTime());
						
						if ( ! Orderbook.isNonDegenerate()) {
								SingletonLogger.Instance().WarningLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" witnesses Orderbook blowup @ "+Scheduler.GetTime()+"\n"+Orderbook.ToStringLong());
						}
						
						IOrder newOrder = null;
						if (DecideToSubmitBid()) {
							
							if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" submits bid @ "+Scheduler.GetTime());
							
							
							// bid
							double price = GetBidPrice();
							int volume = GetBidVolume();
							
							if (volume>0) {
								newOrder = Orderbook.addBid(price, volume, this);
							}
							else {
								if (LoggerDiags.Enabled) SingletonLogger.Instance().WarningLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" attempts unsuccessful zero sized bid @ "+Scheduler.GetTime());
							}
						}
						else {

							if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" submits ask @ "+Scheduler.GetTime());
							
							// ask
							double price = GetAskPrice();
							int volume = GetAskVolume();
							
							if (volume > 0) {
								newOrder = Orderbook.addAsk(price, volume, this);
							}
							else {
								if (LoggerDiags.Enabled) SingletonLogger.Instance().WarningLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" attempts unsuccessful zero sized ask @ "+Scheduler.GetTime());
							}
						}
						
						if (newOrder != null) {
							// may be null if dual volume is 0 preventing creation of new orders
							AddToOpenOrderList(newOrder);
							
							if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" submitted "+newOrder+" @ "+Scheduler.GetTime());
						}
						else {throw new Exception("We got a problem");
						}
					}
				}  
				
				double timeToNextPrompt = GetTimeToNextActionPrompt();
				if (timeToNextPrompt < 0.0) {
					// agent checks out of ecosystem by indicating a negative time
					if (LoggerDiags.Enabled) SingletonLogger.Instance().WarningLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" leaves simulation by indicating negative prompt time @ "+Scheduler.GetTime());
				}
				else {
					this.Send (this, simEvent, timeToNextPrompt);
				}
			}
		}
		
		private void EvaluateAllOpenOrders() {
			IList<IOrder> toUnlist = new List<IOrder>();
			
			foreach (IOrder openOrder in OpenOrders) {
				if (DecideToCancelOpenOrder(openOrder)) {
					
					if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" cancels "+openOrder+" @ "+Scheduler.GetTime());

					toUnlist.Add(openOrder);
					Orderbook.cancelOrder(openOrder);
				}
			}
			
			foreach (IOrder listedOrder in toUnlist) {
				RemoveFromOpenOrderList(listedOrder);
			}
		}
		
	    public override void Destructor() {
		}
		
	    public override string GetName() {
			return BASENAME+"-"+this.ID;
		}
		
		public override string ToString() {
			string str = ""+GetName();
			str += " @ " + Coordinates;
			return str;
		}
		
		private Dictionary<string,double> _metrics;
		
		public void validateMetrics(int expected) {
			if (_metrics.Count != expected) {
				if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" we got a problem with the number of metrics present");
				
				if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+", # of _metrics present: "+_metrics.Count);
				
				foreach (string x in _metrics.Keys) {
					if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" _metric: "+x+" which has value "+_metrics[x]);
				}
				
				throw new Exception("AbstractAgent "+GetName()+" validateMetrics failed, expected "+expected+" but found "+_metrics.Count);
			}
		}
		
		public double GetMetricValue (string metricName)
		{
			if ( ! _metrics.ContainsKey (metricName)) {
				throw new Exception("No metric named: "+metricName);
			}
			return _metrics[metricName];
		}
		
		public Dictionary<string,double>.KeyCollection GetMetrics() {
			return _metrics.Keys;
		}
		
		public bool HasMetric(string metricName) {
			return (_metrics.ContainsKey(metricName));
		}
		
		public void SetMetricValue(string metricName, double val) {
			if (_metrics.ContainsKey(metricName)) {
				_metrics.Remove(metricName);
			}
			_metrics.Add (metricName, val);
			
			if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(AbstractAgent), "AbstractAgent "+GetName()+" sets '"+metricName+"' to: "+val+" @ "+Scheduler.GetTime());
		}
	}
}

