using System;

namespace core
{
	public interface ITrajectoryFactory : INamedObject
	{
		void Initialize(ITableGenerationConfig config);
		
		ITrajectory create();
	}
}

