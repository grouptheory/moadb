using System;

namespace core
{
	public interface IAgentEvaluationBundleCollapser
	{
		IBlauSpaceEvaluation eval(IAgentEvaluationBundle aeb, IBlauSpaceLattice lattice);
	}
}

