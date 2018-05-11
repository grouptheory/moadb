using System;
using System.Collections.Generic;

namespace core
{
	public interface IBlauSpaceIterator : IBlauSpaceLattice, IEnumerable<IBlauPoint>
	{
		bool hasNext();
		IBlauPoint next();
		void reset();
		
		IBlauSpaceIterator clone();
		
	}
}

