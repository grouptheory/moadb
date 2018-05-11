using System;
using core;
using blau;

namespace dist
{
	public class DistributionSpace_SingleGaussian : IDistributionSpace
	{
		private IBlauSpace _space;
		public IBlauSpace BlauSpace {
			get {return _space;}
		}
		
		public IDistributionSpaceIterator iterator() {
			return new DistributionSpaceIterator_SingleGaussian(this.BlauSpace, this.MeanIterator, this.StdIterator);
		}
		
		private IBlauSpaceIterator _meanIterator;
		public IBlauSpaceIterator MeanIterator {
			get {return _meanIterator;}
		}
		
		private IBlauSpaceIterator _stdIterator;
		public IBlauSpaceIterator StdIterator {
			get {return _stdIterator;}
		}
		
		public DistributionSpace_SingleGaussian (IBlauSpace space, IBlauSpaceIterator meanIter, IBlauSpaceIterator stdIter)
		{
			_space = space;
			_meanIterator = meanIter;
			_stdIterator = stdIter;
		}
		
		public override string ToString ()
		{
			string s = "DistributionSpace_SingleGaussian["+_space+"]-(meanSpace:"+_meanIterator.BlauSpace+", "+"stdSpace:"+_stdIterator.BlauSpace+")";
			return s;
		}
	}
}

