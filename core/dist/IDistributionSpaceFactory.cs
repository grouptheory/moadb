using System;

namespace core
{
	public interface IDistributionSpaceFactory
	{
		IBlauSpace SampleSpace {
			get;
		}
		
		IDistributionSpace create();
	}
}

