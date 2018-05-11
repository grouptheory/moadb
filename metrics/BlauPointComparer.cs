using System;
using System.Collections.Generic;
using core;

namespace metrics
{
	public class BlauPointComparer : IBlauPointComparer
	{
		public bool Equals(IBlauPoint b1, IBlauPoint b2)
	    {
			return b1.CompareTo(b2)==0;
	    }
	
	    public int GetHashCode(IBlauPoint bx)
	    {
	        return bx.GetHashCode();
	    }
	}
}

