using System;
using core;

namespace metrics
{
	public class Score : IScore
	{
		private IBlauPoint _coord;
		private double _val;
		
		public Score(IBlauPoint coord, double val) {
			_coord = coord;
			_val = val;
		}
		
		public IBlauPoint Coordinates {
			get { return _coord; }
		}
		
		public double Value {
			get { return _val; }
		}
		
		public override string ToString ()
		{
			string s = "Score ("+Coordinates+" ==> "+Value+")";
			return s;
		}
	}
}

