using System;
using core;
using blau;
using logger;

namespace dist
{
	public class DistributionSpace : IDistributionSpace
	{
		private IDistribution _d;
		private IBlauSpace _parmSpace;
		
		public IDistribution TemplateDistribution {
			get { return _d; }
		}
		
		public IBlauSpace ParamSpace {
			get { return _parmSpace; }
		}
		
		public DistributionSpace (IDistribution d)
		{
			_d = d;
			
			int parms = d.Params;
			string [] names = new string[parms];
			double [] min = new double[parms];
			double [] max = new double[parms];
			
			for (int i=0; i<parms; i++) {
				names[i] = d.getParamName(i);
				min[i] = d.getParamMin(i);
				max[i] = d.getParamMax(i);
			}
			
			_parmSpace = BlauSpace.create (parms, names, min, max);
		}
		
		public IDistributionSpaceIterator iterator(int[] steps) {
			return new DistributionSpaceIterator(this, steps);
		}
	}
}

