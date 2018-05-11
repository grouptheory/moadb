using System;
using System.Collections.Generic;
using blau;
using core;

namespace metrics
{
	public class BlauSpaceMultiEvaluation : IBlauSpaceMultiEvaluation
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
		
		private SortedDictionary<IBlauPoint, LinkedList<IScore>> _evaluationData;

		public BlauSpaceMultiEvaluation (string name, IBlauSpaceLattice lattice)
		{
			_name = name;
			_lattice = lattice;
			_evaluationData = new SortedDictionary<IBlauPoint, LinkedList<IScore>>(new BlauPointComparer());                                                                           
		}
		
		public void set(IBlauPoint p, double val) {
			IBlauPoint qp = Lattice.quantize(p);
			LinkedList<IScore> bin = null;
			if (_evaluationData.ContainsKey(qp)) {
				bin = _evaluationData[qp];
			}
			else {
				bin = new LinkedList<IScore>();
				_evaluationData.Add(qp, bin);
			}
			
			bin.AddLast(new Score(p, val));
		}
		
		public LinkedList<IScore> eval(IBlauPoint bp) {
			IBlauPoint qp = Lattice.quantize(bp);
			if (_evaluationData.ContainsKey(qp)) {
				return _evaluationData[qp];
			}
			throw new Exception("No evaluation found for specified IBlauPoint in BlauSpaceMultiEvaluation!"); 
		}
		
		public override string ToString ()
		{
			string s = "BlauSpaceMultiEvaluation '"+Name+"' w/ "+_evaluationData.Count+" AssignedLatticePoints";
			return s;
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			foreach (IBlauPoint p in _evaluationData.Keys) {
				LinkedList<IScore> scores = _evaluationData[p];
				s += ""+p+" ==> ";
				foreach (IScore sc in scores) {
					s += ("" + sc + "; ");
				}
				s += "\n";
			}
			return s;
		}
	}
}

