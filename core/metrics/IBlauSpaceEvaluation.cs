using System;

namespace core
{
	public interface IBlauSpaceEvaluation : INamedObject, IBlauSpaceLatticedFunction, IPresentable
	{
		void set(IBlauPoint p, double val);
		double eval(IBlauPoint bp);
		
		string ToStringLong ();
	}
}

