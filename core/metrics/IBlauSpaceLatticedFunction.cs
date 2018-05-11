using System;
using System.Collections.Generic;

namespace core
{
	public interface IBlauSpaceLatticedFunction
	{
		IBlauSpaceLattice Lattice {
			get;
		}
		
		LinkedList<IBlauPoint> AssignedLatticePoints {
			get;
		}
	}
}

