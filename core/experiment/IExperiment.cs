using System;
using System.Collections.Generic;

namespace core
{
	public interface IExperiment
	{
		string POP_DIR_STRING {
			get;
		}
		string TRAJ_DIR_STRING {
			get;
		}
		string BSE_DIR_STRING {
			get;
		}
		string RESULTS_DIR_STRING {
			get;
		}

		string Name {
			get;
		}
		
		string OutputDirectory {
			get;
		}
		
		ITableGenerationConfig theTableConfig {
			get ;
		}
		
		ITrajectoryFactorySetConfig theTrajConfig {
			get ;
		}
		
		IAgentEvaluationFactorySetConfig theAgentEvaluationFactorySetConfig {
			get ;
		}
		
		IDistribution theActualDistribution {
			get;
			set;
		}
		
		IBlauSpace theBlauSpace {
			get ;
		}
		
		void run();
		
		ISimulationBundle getSimulationBundle();
		ISimulationResultsBundle getSimulationResultsBundle();
	}
}

