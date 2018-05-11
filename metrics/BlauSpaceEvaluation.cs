using System;
using System.Collections.Generic;
using core;
using blau;

namespace metrics
{
	public class BlauSpaceEvaluation : IBlauSpaceEvaluation
	{
		private string _name;
		public string Name {
			get {return _name;}
		}
		
		private IBlauSpaceLattice _lattice;
		public IBlauSpaceLattice Lattice {
			get {return _lattice;}
		}
		
		public LinkedList<IBlauPoint> AssignedLatticePoints {
			get {
				return new LinkedList<IBlauPoint>(_evaluationData.Keys);
			}
		}
		
		private SortedDictionary<IBlauPoint, double> _evaluationData;

		public BlauSpaceEvaluation (string name, IBlauSpaceLattice lattice)
		{
			_name = name;
			_lattice = lattice;
			_evaluationData = new SortedDictionary<IBlauPoint, double>(new BlauPointComparer());                                                                           
		}
		
		public void set(IBlauPoint p, double val) {
			IBlauPoint qp = Lattice.quantize(p);
			if (_evaluationData.ContainsKey(qp)) {
				throw new Exception("Duplicate assignment for the same IBlauPoint in BlauSpaceEvaluation"); 
			}
			else {
				_evaluationData.Add(qp, val);
			}
		}
		
		public double eval(IBlauPoint bp) {
			IBlauPoint qp = Lattice.quantize(bp);
			if (_evaluationData.ContainsKey(qp)) {
				return _evaluationData[qp];
			}
			throw new Exception("No evaluation found for specified IBlauPoint in BlauSpaceEvaluation!"); 
		}
		
		public override string ToString ()
		{
			string s = "BlauSpaceEvaluation '"+Name+"' w/ "+_evaluationData.Count+" AssignedLatticePoints";
			return s;
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			foreach (IBlauPoint p in _evaluationData.Keys) {
				double val = _evaluationData[p];
				s += ""+p+" ==> "+val+"\n";
			}
			return s;
		}
	}
}

