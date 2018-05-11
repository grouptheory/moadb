using System;
using System.Collections.Generic;
using core;
//using blau;

namespace metrics
{
	public class AgentEvaluation : IAgentEvaluation
	{
		private string _name;
		public string Name {
			get {return _name;}
		}
		
		private IAgentEvaluationFactory _creator;
		public IAgentEvaluationFactory Creator {
			get {return _creator;}
		}
		
		public AgentEvaluation (string name, IAgentEvaluationFactory creator)
		{
			_name = name;
			_creator = creator;
			_evaluationData = new Dictionary<IAgent, double>();                                                                           
		}
		
		private Dictionary<IAgent, double> _evaluationData;
		
		public bool Valid {
			get { return _evaluationData.Count!=0; }
		}
		
		public void set(IAgent ag, double val) {
			if (_evaluationData.ContainsKey(ag)) {
		        throw new Exception("Attempt to register multiple evaluations for the same Agent!");  
			}
			else {
				_evaluationData.Add(ag, val);
			}
		}
		
		public double eval(IAgent ag) {
			if (_evaluationData.ContainsKey(ag)) {
				return _evaluationData[ag];
			}
			throw new Exception("No evaluation found for specified IAgent in AgentEvaluation!"); 
		}
		
		public void AddToBlauSpaceMultiEvaluation(IBlauSpaceMultiEvaluation bse) {
			if ( ! bse.Name.Equals(this.Name)) {
				throw new Exception("Attempt to AddToBlauSpaceEvaluation with two incompatible AgentEvaluation/IBlauSpaceEvaluation"); 
			}
			
				// Console.WriteLine("XXX bse.Lattice "+bse.Lattice);
			
			foreach (IAgent ag in _evaluationData.Keys) {
				IBlauPoint p = ag.Coordinates;
				IBlauPoint pq = bse.Lattice.quantize (p);
				
				// Console.WriteLine("XXX p.dim:"+p.Space.Dimension+"   pq.dim:"+pq.Space.Dimension+"   bse.Lattice.dim:"+bse.Lattice.BlauSpace.Dimension);
				// Console.WriteLine("XXX pt "+p+" ==Q==> "+pq);
				
				double val = _evaluationData[ag];
				bse.set(pq, val);
			}
		}
		
		public override string ToString ()
		{
			string s = "AgentEvaluation '"+Name+"' for "+_evaluationData.Count+" Agents";
			return s;
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			foreach (IAgent ag in _evaluationData.Keys) {
				s += ""+ag+" ==> ";
				double val = _evaluationData[ag];
				s += ""+val;
				s += "\n";
			}
			return s;
		}
	}
}

