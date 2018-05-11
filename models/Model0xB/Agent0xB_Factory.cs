using System;
using core;
using agent;

namespace models
{
	public class Agent0xB_Factory : AbstractAgentFactory
	{
		protected override IAgent create(IBlauPoint pt, IAgentFactory creator, int id) {
			return new Agent0xB(pt, creator, id);
		}

		protected override bool ValidateDistribution(IDistribution dist) {
			bool ok = true;
			ok = ok && ValidateSampleSpace(dist.SampleSpace);
			return ok;
		}
		
		public Agent0xB_Factory(IDistribution dist) : base(dist) {}
		
		public override void Initialize(ITableGenerationConfig config) {
			// no op
		}
	}
}

