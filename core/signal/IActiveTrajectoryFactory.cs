using System;

namespace core
{
	public interface IActiveTrajectoryFactory: ITrajectoryFactory
	{
		void reset(ISimulationParameters sim);
	}
}

