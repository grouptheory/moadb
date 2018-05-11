using System;
using core;
using agent;
using logger;

namespace models
{
	public class Agent0x1_Factory : AbstractAgentFactory
	{
		protected override IAgent create(IBlauPoint pt, IAgentFactory creator, int id) {
			SingletonLogger.Instance().InfoLog(typeof(Agent0x1), "Agent0x1_Factory creating agent "+id);
			IAgent dupe = new Agent0x1(pt, creator, id);
			return dupe;
		}

		protected override bool ValidateDistribution(IDistribution dist) {
			bool ok = true;
			ok = ok && ValidateSampleSpace(dist.SampleSpace);
			return ok;
		}
		
		public Agent0x1_Factory(IDistribution dist) : base(dist) {}
		
		public override void Initialize(ITableGenerationConfig config) {
			// no op
		}
	}
}

