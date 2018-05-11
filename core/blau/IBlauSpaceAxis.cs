using System;

namespace core
{
	public interface IBlauSpaceAxis
	{
		string Name {
			get;
		}
		
		double MinimumValue {
			get;
		}
		
		double MaximumValue {
			get;
		}
	}
}

