using System;
using System.Collections.Generic;
using core;
using agent;
using logger;

namespace agent
{
	/// <summary>
    /// AbstractAgentFactory class
    /// </summary>
	public abstract class AbstractAgentFactory : IAgentFactory
	{
		
		// private factory method
		protected abstract IAgent create(IBlauPoint pt, IAgentFactory creator, int id);
		
		// the factory is told about the global configuration objects
		public abstract void Initialize(ITableGenerationConfig config);
		
		// validate the distribution
		protected abstract bool ValidateDistribution(IDistribution dist);
		
		// the distribution by which agents are placed in blauspace
		private IDistribution _dist;
		public IDistribution Distribution {
			get { return _dist; }
		}
		
		// required blauspace axes
		private List<string> _requiredAxes = new List<string>();
		
		// constructor
		protected AbstractAgentFactory (IDistribution dist)
		{
			if (dist == null) {
				throw new Exception("AbstractAgentFactory received a NULL distribution!");
			}
			
			_dist = dist;

			if ( ! ValidateDistribution(dist)) {
				throw new Exception("AbstractAgentFactory received an invalid distribution: "+dist);
			}

			if (  ! ValidateSampleSpace(dist.SampleSpace)) {
				throw new Exception("AbstractAgentFactory received a distribution with an invalid sample space: "+dist.SampleSpace);
			}
		}
		
		// create an agent
		public IAgent create() {
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgentFactory), "AbstractAgentFactory sampling a BlauPoint...");
			IBlauPoint pt = _dist.getSample();
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(AbstractAgentFactory), "AbstractAgentFactory done sampling, chose: "+pt);
			
			// dynamic ID allocation
			return create(pt, this, -1);
		}
		
		// clone an agent
		public virtual IAgent cloneAgent(IAgent orig) {
			if ( orig.Creator != this ) {
				throw new Exception("Only the creator of an Agent may clone it!");
			}
			
			// clone point
			IBlauPoint pt = orig.Coordinates.clone();
			// explicit ID allocation
			IAgent dupe = create(pt, this, orig.ID);
			
			// duplicate metrics
			foreach (string metricName in orig.GetMetrics()) {
				dupe.SetMetricValue( metricName, orig.GetMetricValue(metricName) );
			}
			
			return dupe;
		}
		
		// add a required axis
		protected void AddRequiredAxis(string s) {
			if ( ! _requiredAxes.Contains(s)) {
				_requiredAxes.Add(s);
			}
		}
		
		// validate blauspace for required axes
		protected bool ValidateSampleSpace(IBlauSpace space) {
			bool valid = true;
			
			foreach (string s in _requiredAxes) {
				if ( ! _dist.SampleSpace.hasAxis(s)) {
					if (LoggerDiags.Enabled) SingletonLogger.Instance().ErrorLog(typeof(AbstractAgentFactory), "AbstractAgentFactory requires "+s+" but it is not present in the BlauSpace of distribution "+_dist);
					valid = false;
				}
			}

			/*
			if (_requiredAxes.Count != _dist.SampleSpace.Dimension) {
				if (LoggerDiags.Enabled) SingletonLogger.Instance().WarningLog(typeof(AbstractAgentFactory), "AbstractAgentFactory requires "+_requiredAxes.Count+" but the BlauSpace has dimension: "+_dist.SampleSpace.Dimension);
				valid = false;
			}
			*/

			return valid;
		}
		
		// print the factory to string
		public override string ToString ()
		{
			string s = ""+this.GetType().Name+"["+_dist.SampleSpace+"]-dist:"+_dist;
			return s;
		}
	}
}

