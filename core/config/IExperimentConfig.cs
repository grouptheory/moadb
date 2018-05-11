using System;

namespace core
{
	public interface IExperimentConfig
	{
		int NumAgents {
			get;
		}
		double DurationHours {
			get;
		}
		int Trials {
			get;
		}
		string InitialOrderbook {
			get;
		}
	}
}

