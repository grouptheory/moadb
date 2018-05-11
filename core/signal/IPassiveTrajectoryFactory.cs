using System;

namespace core
{
	public interface IPassiveTrajectoryFactory: ITrajectoryFactory, ISimulationObserver
	{
		void reset();
	}
}

