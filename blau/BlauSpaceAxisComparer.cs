using System;
using System.Collections.Generic;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace blau
{
	/// <summary>
    /// BlauSpaceAxisComparer class
    /// </summary>
	class BlauSpaceAxisComparer : IComparer<BlauSpaceAxis> {
		
		// comparator of blauspaces axes, for use in treemaps, and ultimately in 
		// sorting BlauSpaceAxis objects so as to compare canonicalized forms of BlauSapce
		public int Compare(BlauSpaceAxis x, BlauSpaceAxis y) {
			int c1 = x.Name.CompareTo (y.Name) ;
			if (c1 != 0) return c1;
			else {
				int c2 = (x.MinimumValue < y.MinimumValue) ? -1 : ((x.MinimumValue > y.MinimumValue) ? +1 : 0);
				if (c2 != 0) return c2;
				int c3 = (x.MaximumValue < y.MaximumValue) ? -1 : ((x.MaximumValue > y.MaximumValue) ? +1 : 0);
				return c3;
			}
		}
	}
}

