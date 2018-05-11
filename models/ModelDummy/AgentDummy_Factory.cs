using System;
using core;
using agent;

namespace models
{
	public class AgentDummy_Factory : AbstractAgentFactory
	{
		protected override IAgent create(IBlauPoint pt, IAgentFactory creator, int id) {
			return new AgentDummy(pt, creator, id);
		}

		protected override bool ValidateDistribution(IDistribution dist) {
			bool ok = true;
			ok = ok && ValidateSampleSpace(dist.SampleSpace);
			return ok;
		}
		
		public AgentDummy_Factory(IDistribution dist) : base(dist) {}
		
		public override void Initialize(ITableGenerationConfig config) {
			// no op
		}
	}
}

