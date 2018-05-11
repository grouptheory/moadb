using System;
using core;
using logger;

namespace signal
{
	public abstract class AbstractActiveTrajectoryFactory : IActiveTrajectoryFactory
	{
		private double _period;
		protected double Period {
			get { return _period; }
		}
		
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
		
		private ITrajectory _trajectory;
		protected ITrajectory MyTrajectory {
			get { return _trajectory; }
		}

		private double _burnin;
		protected double BurninTime {
			get { return _burnin; }
		}
		
		public void Initialize(ITableGenerationConfig config) {
			_burnin = config.InitialBurninHours;
			_trajectory = new Trajectory(this.Name, TemporalGranularityThreshold, _historicalBias, _burnin);
		}
		
		public ITrajectory create() {
			ITrajectory answer = MyTrajectory;
			return answer;
		}
		
		public void reset(ISimulationParameters sim) {
			_trajectory = new Trajectory(this.Name, TemporalGranularityThreshold, _historicalBias, _burnin);
			_sim  =sim;
		}
		
		private double _temporalGranularityThreshold;
		public double TemporalGranularityThreshold {
			get { return _temporalGranularityThreshold;}
		}
		
		private double _historicalBias;
		
		
		protected AbstractActiveTrajectoryFactory (double timeQuantum, double historicalBias, double period)
		{
			_temporalGranularityThreshold = timeQuantum;
			_historicalBias = historicalBias;
			_period = period;
			_burnin = 0.0;
		}
		
		public abstract string Name {
			get;
		}
		
		public override string ToString ()
		{
			string s = "AbstractActiveTrajectoryFactory '"+Name+"' w/ "+MyTrajectory;
			return s;
		}
	}
}


