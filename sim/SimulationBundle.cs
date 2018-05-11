using System;
using System.Collections.Generic;
using core;
using des;
using logger;
using blau;

namespace sim
{
	public class SimulationBundle : ISimulationBundle
	{
		private IPopulation _pop;
		private IOrderbook_Observable _ob;
		private double _starttime;
		private double _duration;
		private bool _addLogger;
		private string _tag;

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
			get {return Scheduler.GetTime();}
		}
		
		public ISimulationResultsBundle run(int trials) {

			ISimulationResultsBundle resbundle = new SimulationResultsBundle();

			for (int i=0; i<trials; i++) {
				SingletonLogger.Instance().DebugLog(typeof(SimulationBundle), "Running Simulation trial "+i);
				ISimulation sim2 = MakeSimulation(i, trials);

				// Console.WriteLine("FFF Running trial "+i);
				// Console.WriteLine("FFF Simulation pop size "+sim2.Population.Size);

				ISimulationResults res2 = sim2.run();
				// Console.WriteLine("FFF Done trial "+i);

				SingletonLogger.Instance().DebugLog(typeof(SimulationBundle), "Completed Simulation trial "+i);
				resbundle.add(res2);
			}

			return resbundle;
		}
		
		private Dictionary<string, IPassiveTrajectoryFactory> _name2tf;
		private Dictionary<string, IAgentEvaluationFactory> _name2af;
		
		public void add(IPassiveTrajectoryFactory tf) {
			if (!_name2tf.ContainsKey(tf.Name)) {
				_name2tf.Add(tf.Name, tf);
			}
		}
		
		Dictionary<IAgentEvaluationFactory, IAgentEvaluationConfig> _aef2conf = 
			new Dictionary<IAgentEvaluationFactory, IAgentEvaluationConfig>();
			
		public void add(IAgentEvaluationFactory evf, IAgentEvaluationConfig conf) {
			if (!_name2af.ContainsKey(evf.Name)) {
				_name2af.Add(evf.Name, evf);
				_aef2conf.Add (evf, conf);
			}
		}
		
		public IAgentEvaluationConfig getAgentEvaluationConfig(IAgentEvaluationFactory evf) {
			return _aef2conf[evf];
		}
		
		public IEnumerable<IPassiveTrajectoryFactory> getPassiveTrajectoryFactories() {
			return _name2tf.Values;
		}
		
		public IEnumerable<IAgentEvaluationFactory> getAgentEvaluationFactories(){
			return _name2af.Values;
		}
		
		private ISimulation MakeSimulation (int trial, int totalTrials)
		{
			IPopulation pop2;

			// WE NO LONGER DO ANYTHING BASED ON trial versus totalTrials
			pop2 = _pop.clone ();
			/*
			if (trial < totalTrials - 1) {
				pop2 = _pop.clone ();
				Console.WriteLine("FFF Trial "+trial+" cloning the master pop");
			} else if (trial == totalTrials - 1) {
				pop2 = _pop;
				Console.WriteLine("FFF Trial "+trial+" using the master pop");
			} else {
				throw new Exception ("Trial number exceeds total number of trials");
			}
			*/

			IOrderbook_Observable ob2 = _ob.clone();
			double time2 = _starttime;
			double duration2 = _duration;
			
			ISimulation sim2 = new Simulation(pop2, ob2, time2, duration2, _addLogger, _tag+"-trial-"+trial);
			
			foreach (IPassiveTrajectoryFactory tf in getPassiveTrajectoryFactories()) {
				sim2.add(tf);
			}
			foreach (IAgentEvaluationFactory aef in getAgentEvaluationFactories()) {
				sim2.add(aef);
			}
			return sim2;
		}
		
		public SimulationBundle (IPopulation pop, IOrderbook_Observable ob, double starttime, double duration, bool addLogger, string tag)
		{
			_pop = pop;
			_ob = ob;
			_starttime = starttime;
			_duration = duration;
			_addLogger = addLogger;
			_tag = tag;

			_name2tf = new Dictionary<string, IPassiveTrajectoryFactory>();
			_name2af = new Dictionary<string, IAgentEvaluationFactory>();
		}
		
		public override string ToString ()
		{
			string s = "SimulationBundle ["+StartTime+"-"+(StartTime+Duration)+"] ==> ";
			s += "OB asks:"+Orderbook.getNumAsks()+", bids:"+Orderbook.getNumBids()+"; ";
			s += "Pop:"+Population.Size;
			return s;
		}
		
		public IBlauSpaceLattice getLattice(IDistribution d, IAgentEvaluationBundle aeb) {
			IAgentEvaluationFactory aef = null;;
			foreach (IAgentEvaluation iae in aeb.Evaluations) {
				aef = iae.Creator;
				break;
			}
			
			int[] steps = new int[d.SampleSpace.Dimension]; 
			for (int j=0;j<d.SampleSpace.Dimension;j++) steps[j]=getAgentEvaluationConfig(aef).BlauSpaceGridding;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(d.SampleSpace, steps);
			return bsl;
		}
	}
}

