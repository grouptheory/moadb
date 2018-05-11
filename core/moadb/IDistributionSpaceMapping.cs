using System;

namespace core
{
	public interface IDistributionSpaceMapping
	{
		void addMeanTrajectory(IDistribution dist, ITrajectory mean);
		void addDivergenceTrajectory(IDistribution dist, ITrajectory div);
		void addBlauSpaceEvaluation(IDistribution dist, IAgentEvaluationBundle ev);
		
		ITrajectory getMeanTrajectory(IDistribution dist);
		ITrajectory getDivergenceTrajectory(IDistribution dist);
		IAgentEvaluationBundle getBlauSpaceEvaluation(IDistribution dist);
	}
}

