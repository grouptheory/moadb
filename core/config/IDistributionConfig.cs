using System;
using System.Collections.Generic;

namespace core
{
	public interface IDistributionConfig
	{
		string SpecificDistributionConfigClassName {
			get;
		}
		
		List<IBlauAxisDistributionConfig> getAxes();
		
		bool ValidateDistributionCoverage(IBlauSpaceConfig bsc);
	}
}

