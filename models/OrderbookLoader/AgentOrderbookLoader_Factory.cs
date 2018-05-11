using System;
using core;
using agent;

namespace models
{
	public class AgentOrderbookLoader_Factory : AbstractAgentFactory
	{
		protected override IAgent create(IBlauPoint pt, IAgentFactory creator, int id) {
			return new AgentOrderbookLoader(_path, pt, creator, id);
		}

		protected override bool ValidateDistribution(IDistribution dist) {
			bool ok = true;
			ok = ok && ValidateSampleSpace(dist.SampleSpace);
			return ok;
		}
		
		private string _path;
		
		public AgentOrderbookLoader_Factory(string path, IDistribution dist) : base(dist) {
			_path = path;
		}
		
		public override void Initialize(ITableGenerationConfig config) {
			// no op
		}
	}
}


