using System;

namespace core
{
	public interface ITrajectoryBundleCollapser
	{	
		ITrajectory eval(ITrajectoryBundle tb);
	}
}

