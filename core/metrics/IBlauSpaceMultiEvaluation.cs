using System;
using System.Collections.Generic;

namespace core
{
	public interface IBlauSpaceMultiEvaluation : INamedObject, IBlauSpaceLatticedFunction
	{
		void set(IBlauPoint p, double val);
		LinkedList<IScore> eval(IBlauPoint bp);
		
		string ToStringLong ();
	}
}

