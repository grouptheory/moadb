using System;

namespace core
{
	public interface IAgentEvaluationConfig
	{
		string Name {
			get;
		}
		string MetricName {
			get;
		}
		int BlauSpaceGridding {
			get;
		}
	}
}

