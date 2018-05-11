using System;

namespace core
{
	public interface IDistributionSpace
	{
		IDistribution TemplateDistribution {
			get ;
		}
		
		IBlauSpace ParamSpace {
			get;
		}
		
		IDistributionSpaceIterator iterator(int[] steps);
	}
}

