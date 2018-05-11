using System;
using System.Collections.Generic;
using core;

namespace metrics
{
	public class AgentEvaluationBundleCollapser_Std : IAgentEvaluationBundleCollapser
	{
		private static IAgentEvaluationBundleCollapser _instance;
		
		public static IAgentEvaluationBundleCollapser Instance() {
			if (_instance == null) {
				_instance = new AgentEvaluationBundleCollapser_Std();
			}
			return _instance;
		}
		
		private AgentEvaluationBundleCollapser_Std ()
		{
		}
		
		public IBlauSpaceEvaluation eval(IAgentEvaluationBundle aeb, IBlauSpaceLattice lattice) {
			
			IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(lattice);
			
			IBlauSpaceMultiEvaluation bsme = new BlauSpaceMultiEvaluation(aeb.Name, lattice);
			IBlauSpaceEvaluation bse = new BlauSpaceEvaluation(aeb.Name+"-Std", lattice);
			foreach (IAgentEvaluation ae in aeb.Evaluations) {
				ae.AddToBlauSpaceMultiEvaluation(bsme);
			}
			foreach (IBlauPoint p in bsme.AssignedLatticePoints) {
				LinkedList<IScore> scores = bsme.eval(p);
				double meanValue = meanEval.eval(p);
				
				double total = 0.0;
				int count = 0;
				foreach (IScore s in scores) {
					double delta = (s.Value - meanValue);
					total += (delta*delta);
					count++;
				}
				double mean = Math.Sqrt(total)/(double)count;
				bse.set(p, mean);
			}
			return bse;
		}
	}
}

