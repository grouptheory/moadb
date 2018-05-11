using System;
using System.Collections.Generic;
using core;
using des;
using logger;
using orderbook;

namespace sim
{
	public class Simulation : SimEntity, ISimulation
	{
		// SimEntity
		
	    public override void Destructor() {
			// no op
		}
		
	    public override string GetName() {
			return "Simulation";
		}
	
	    public override void Recv(ISimEntity src, ISimEvent simEvent) {
			// no op
			// throw new Exception("Unexpected call to Simulation.Recv");
		}
		
		private IPopulation _pop;
		private IOrderbook_Observable _ob;
		private double _starttime;
		private double _duration;
		
		public IPopulation Population {
			get {return _pop;}
		}
		
		public IOrderbook_Agent Orderbook {
			get {return _ob;}
		}
		
		public double StartTime {
			get {return _starttime;}
		}
		
		public double Duration {
			get {return _duration;}
		}
		
		public double Time {
			get {
				if (_stage == PRE_RUN) {
					return _starttime;
				}
				else if (_stage == RUNNING) {
					return Scheduler.GetTime();
				}
				else {
					return _starttime+_duration;
				}
			}
		}
		
		public void recvOrderbookNotification(IOrderbook_Matcher ob, IOrderbookEvent evt) {
			
			SingletonLogger.Instance().DebugLog(typeof(Simulation), "recvOrderbookNotification");
			
			if (ob == _ob) {
				SimulationEvent se = new SimulationEvent(evt);
				if (evt is IOrderbookEvent_AddOrder) {
					notifyAllTrajectoryFactories(se);
					notifyAllAgentEvaluationFactories(se);
				}
				else if (evt is IOrderbookEvent_CancelOrder) {
					notifyAllTrajectoryFactories(se);
					notifyAllAgentEvaluationFactories(se);
				}
				else if (evt is IOrderbookEvent_FillOrder) {
					SingletonLogger.Instance().DebugLog(typeof(Simulation), "IOrderbookEvent_FillOrder");
					notifyPopulation(se);
					notifyAllTrajectoryFactories(se);
					notifyAllAgentEvaluationFactories(se);
				}
			}
		}

		public Simulation (IPopulation pop, IOrderbook_Observable ob, double starttime, double duration, bool addLogger, string tag) 
		{
			_pop = pop;
			_ob = ob;
			_starttime = starttime;
			_duration = duration;

			_ob.addObserver(this);
			if (addLogger) _ob.addObserver(new OrderbookLogger(tag));

			_tflist = new List<IPassiveTrajectoryFactory>();
			_aflist = new List<IAgentEvaluationFactory>();
		}

		private static readonly int PRE_RUN = 1;
		private static readonly int RUNNING = 2;
		private static readonly int POST_RUN = 3;

		private int _stage = PRE_RUN;

		public ISimulationResults run() {
			
			this.Send(this, new SchedulerStopSimEvent(), _duration);
			Scheduler.Instance().InitializeStartTime(_starttime);
			
			ISimulationEvent startEvent = new SimulationStart();
			broadcast(startEvent);

			_stage = RUNNING;
			Scheduler.Instance().Run();
			_stage = POST_RUN;

			ISimulationEvent endEvent = new SimulationEnd();
			
			// set final price to current price
			_ob.FinalPrice = _ob.getPrice ();
			
			broadcast(endEvent);
			
			ISimulationResults results = new SimulationResults();
			foreach (IAgentEvaluationFactory af in _aflist) {
				results.add(af.create());
			}
			foreach (ITrajectoryFactory tf in _tflist) {
				if ( ! (tf is ITrajectoryFactory_Ignore)) {
					results.add(tf.create());
				}
			}
			return results;
		}
	
		public void add(IPassiveTrajectoryFactory tf) {
			_tflist.Add(tf);
		}
		
		public void add(IAgentEvaluationFactory af) {
			_aflist.Add(af);
		}
	
		public IEnumerator<IPassiveTrajectoryFactory> getPassiveTrajectoryFactories() {
			return _tflist.GetEnumerator();
		}
		
		public IEnumerator<IAgentEvaluationFactory> getAgentEvaluationFactories() {
			return _aflist.GetEnumerator();
		}
		
		private List<IPassiveTrajectoryFactory> _tflist;
		private List<IAgentEvaluationFactory> _aflist;
		
		public void broadcast(ISimulationEvent se) {
			notifyPopulation(se);
			notifyAllAgentEvaluationFactories(se);
			notifyAllTrajectoryFactories(se);
		}
		
		private void notifyAllTrajectoryFactories(ISimulationEvent se) {
			foreach (IPassiveTrajectoryFactory tf in _tflist) {
					SingletonLogger.Instance().DebugLog(typeof(Simulation), "notifyAllTrajectoryFactories");
				tf.recvSimulationNotification(this, se);
			}
		}
		private void notifyAllAgentEvaluationFactories(ISimulationEvent se) {
			foreach (IAgentEvaluationFactory af in _aflist) {
				af.recvSimulationNotification(this, se);
			}
		}
		private void notifyPopulation(ISimulationEvent se) {
			_pop.recvSimulationNotification(this, se);
		}
		
		public override string ToString ()
		{
			string s = "*** Simulation *** ";
			s += "Time:"+Time+" ["+StartTime+"-"+(StartTime+Duration)+"] ==> ";
			s += ""+Orderbook+"; ";
			s += "Pop:"+Population.Size;
			return s;
		}
	}
}

