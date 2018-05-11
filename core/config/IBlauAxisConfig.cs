using System;

namespace core
{
	public interface IBlauAxisConfig
	{
		int Index {
			get;
		}
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

