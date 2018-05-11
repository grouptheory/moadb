using System;
using System.Collections.Generic;

namespace core
{
	public interface IDistributionSpaceIterator : IEnumerable<IDistribution>
	{
		IDistributionSpace Space {
			get;
		}
		
		bool hasNext();
		IDistribution next();
		void reset();
	}
}

