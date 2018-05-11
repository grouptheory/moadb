using System;
using System.Collections.Generic;
using core;

namespace metrics
{
	public class AgentEvaluationBundle : IAgentEvaluationBundle
	{
		private string _name;
		public string Name {
			get {return _name;}
		}
		
		public AgentEvaluationBundle(string name)
		{
			_name = name;
			_agentevaluations = new LinkedList<IAgentEvaluation> ();
		}
		
		private LinkedList<IAgentEvaluation> _agentevaluations;
		
		public bool addAgentEvaluation(IAgentEvaluation ae) {
			if ( ! ae.Name.Equals(this.Name)) {
				throw new Exception("nonhomogenous AgentEvaluationBundle are not supported");
			}
			_agentevaluations.AddLast(ae);
			return true;
		}
		
		public IBlauSpaceEvaluation AssignmentCounts(IBlauSpaceLattice lattice) {
			IAgentEvaluationBundleCollapser countsCalculator = AgentEvaluationBundleCollapser_Counts.Instance();
			IBlauSpaceEvaluation counts = countsCalculator.eval(this, lattice);
			return counts; 
		}
		
		public IBlauSpaceEvaluation MeanEvaluation(IBlauSpaceLattice lattice) {
			IAgentEvaluationBundleCollapser meanCalculator = AgentEvaluationBundleCollapser_Mean.Instance();
			IBlauSpaceEvaluation mean = meanCalculator.eval(this, lattice);
			return mean; 
		}
		
		public  IBlauSpaceEvaluation StdEvaluation(IBlauSpaceLattice lattice) {
			IAgentEvaluationBundleCollapser stdCalculator = AgentEvaluationBundleCollapser_Std.Instance();
			IBlauSpaceEvaluation std = stdCalculator.eval(this, lattice);
			return std; 
		}
		
		public  bool Valid {
			get {
				foreach (IAgentEvaluation ae in _agentevaluations) {
					if (!ae.Valid) return false;
				}
				return true;
			}
		}
		
		public List<IAgentEvaluation> Evaluations {
			get { 
				List<IAgentEvaluation> list = new List<IAgentEvaluation>();
				list.AddRange(_agentevaluations);
				return list;
			}
		}
		
		public override string ToString ()
		{
			string s = "AgentEvaluationBundle '"+Name+"' w/ "+_agentevaluations.Count+" AgentEvaluations";
			return s;
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			foreach (IAgentEvaluation ae in _agentevaluations) {
				s += ""+ae.ToStringLong()+"\n";
			}
			return s;
		}
	}
}

