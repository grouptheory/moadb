using System;

namespace core
{
	public interface ISequentialDistributionConfig2 : ISpecificDistributionConfig
	{
		double Step {
			get;
		}
	}
}

