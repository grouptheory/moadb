using System;
using System.Collections.Generic;
using core;

namespace metrics
{
	public class AgentEvaluationBundleCollapser_Mean : IAgentEvaluationBundleCollapser
	{
		private static IAgentEvaluationBundleCollapser _instance;
		
		public static IAgentEvaluationBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new AgentEvaluationBundleCollapser_Mean();
			}
			return _instance;
		}
		
		private AgentEvaluationBundleCollapser_Mean ()
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
				double total = 0.0;
				int count = 0;
				foreach (IScore s in scores) {
					total += s.Value;
					count++;
				}
				double mean = total/(double)count;
				bse.set(p, mean);
			}
			return bse;
		}
	}
}

