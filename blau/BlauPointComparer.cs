using System;
using System.Collections.Generic;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace blau
{	
	/// <summary>
    /// BlauPointComparer class
    /// </summary>
	public class BlauPointComparer : IComparer<IBlauPoint> {
		
		// comparator of blauspaces axes, for use in treemaps
		public int Compare(IBlauPoint x, IBlauPoint y) {
		
			if (x.Space.Dimension != y.Space.Dimension) {
				throw new Exception("Attempt to compare incomparable dimensioned BlauPoints");
			}
			
			for (int i=0;i<x.Space.Dimension; i++) {
				string name = x.Space.getAxis(i).Name;
				if ( ! y.Space.hasAxis(name)) {
					throw new Exception("Attempt to compare incomparable BlauPoints: axis "+name);
				}
				int j = y.Space.getAxisIndex(name);
				
				double vi = x.getCoordinate(i);
				double vj = y.getCoordinate(j);
				if (vi < vj) return -1;
				else if (vi > vj) return +1;
			}
			return 0;
		}
	}
	
}

