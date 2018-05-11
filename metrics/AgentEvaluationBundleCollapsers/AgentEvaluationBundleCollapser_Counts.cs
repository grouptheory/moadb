using System;
using System.Collections.Generic;
using core;

namespace metrics
{
	public class AgentEvaluationBundleCollapser_Counts : IAgentEvaluationBundleCollapser
	{
		private static IAgentEvaluationBundleCollapser _instance;
		
		public static IAgentEvaluationBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new AgentEvaluationBundleCollapser_Counts();
			}
			return _instance;
		}
		
		private AgentEvaluationBundleCollapser_Counts ()
		{
		}
		
		public IBlauSpaceEvaluation eval(IAgentEvaluationBundle aeb, IBlauSpaceLattice lattice) {
			IBlauSpaceMultiEvaluation bsme = new BlauSpaceMultiEvaluation(aeb.Name, lattice);
			IBlauSpaceEvaluation bse = new BlauSpaceEvaluation(aeb.Name+"-Mean", lattice);
			foreach (IAgentEvaluation ae in aeb.Evaluations) {
				ae.AddToBlauSpaceMultiEvaluation(bsme);
			}
			foreach (IBlauPoint p in bsme.AssignedLatticePoints) {
				LinkedList<IScore> scores = bsme.eval(p);
				double count = (double)scores.Count;
				bse.set(p, (double)count);
			}
			return bse;
		}
	}
}

