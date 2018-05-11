using System;

namespace core
{
	public interface IBlauSpaceLattice
	{
		IBlauSpace BlauSpace {
			get;
		}
		
		int getSteps(int dim);
		double getStepSize(int dim);
		
		IBlauPoint quantize(IBlauPoint p);
	}
}

