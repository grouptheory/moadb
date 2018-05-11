using System;

namespace core
{
	public interface IBlauPoint : IComparable
	{
		IBlauSpace Space {
			get;
		}
		
		double getCoordinate(int i);
		void setCoordinate(int i, double val);
		
		void setImmutable();
		
		IBlauPoint clone();
	}
}

