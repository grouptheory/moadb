using System;

namespace core
{
	public interface IGaussianDistributionConfig2 : ISpecificDistributionConfig
	{
		IBlauPointConfig getMean();
		IBlauPointConfig getStd();
	}
}

