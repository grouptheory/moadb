using System;

namespace core
{
	public interface IMetaExperiment 
	{
		string DIST_DIR_STRING {
			get;
		}

		IDistribution theTemplateDistribution {
			get ;
		}
		
		IDistributionSpaceIteratorSpecification theDistributionSpaceIteratorSpecification {
			get ;
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
		
		IBlauSpace theBlauSpace {
			get ;
		}
		
		void run();
	}
}

