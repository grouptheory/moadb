using System;

namespace core
{
	public interface ITrajectoryDistanceMeasure
	{
		ITrajectory eval(ITrajectory t1, ITrajectory t2);
		double eval(ITrajectory t1, ITrajectory t2, double t);
	}
}

