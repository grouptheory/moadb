using System;
using System.Collections.Generic;

namespace core
{
	public interface IAgentEvaluationBundle : INamedObject
	{
		bool Valid {
			get;
		}
		
		bool addAgentEvaluation(IAgentEvaluation ae);
		
		List<IAgentEvaluation> Evaluations {
			get;
		}
		
		IBlauSpaceEvaluation MeanEvaluation(IBlauSpaceLattice lattice);
		IBlauSpaceEvaluation StdEvaluation(IBlauSpaceLattice lattice);
		IBlauSpaceEvaluation AssignmentCounts(IBlauSpaceLattice lattice);
		
		string ToStringLong ();
	}
}

