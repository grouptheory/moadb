using System;

namespace core
{
	public interface ITemporalObject
	{
		double MinimumTime {
			get;
		}
		
		double MaximumTime {
			get;
		}
	}
}

