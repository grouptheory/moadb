using System;

namespace core
{
	public interface IScore
	{
		IBlauPoint Coordinates {
			get;
		}
			
		double Value {
			get;
		}
	}
}

