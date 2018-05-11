using System;

namespace core
{
	public interface ITrajectoryTransformer
	{		
		ITrajectory eval(ITrajectory orig);
	}
}

