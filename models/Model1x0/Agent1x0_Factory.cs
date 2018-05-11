using System;
using core;
using agent;
using logger;

namespace models
{
	public class Agent1x0_Factory : AbstractAgentFactory
	{
		protected override IAgent create(IBlauPoint pt, IAgentFactory creator, int id) {
			SingletonLogger.Instance().InfoLog(typeof(Agent1x0), "Agent1x0_Factory creating agent "+id);
			IAgent dupe = new Agent1x0(pt, creator, id, _lambda, _gamma, _burnin);
			return dupe;
		}

		protected override bool ValidateDistribution(IDistribution dist) {
			bool ok = true;
			ok = ok && ValidateSampleSpace(dist.SampleSpace);
			return ok;
		}
		
		public Agent1x0_Factory(IDistribution dist) : base(dist) {
			_lambda = DEFAULT_LAMBDA;
			_gamma = DEFAULT_GAMMA;
			_burnin = DEFAULT_BURNIN;
		}
		
		private static readonly double DEFAULT_LAMBDA = 0.75;
		private double _lambda;
		
		private static readonly double DEFAULT_GAMMA = 0.5;
		private double _gamma;
		
		private static readonly double DEFAULT_BURNIN = 0.0;
		private double _burnin;
		
		public override void Initialize(ITableGenerationConfig config) {
			_lambda = config.Lambda;
			_gamma = config.Gamma;
			_burnin = config.InitialBurninHours;
		}
	}
}

