using System;
using System.Collections.Generic;

namespace core
{
	public interface IDistributionSpaceIteratorSpecification
	{
		int[] ApplyToDistribution(IDistribution d);
	}
}

