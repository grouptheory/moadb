using System;
using System.Collections.Generic;
using core;
using blau;

namespace dist
{
	public class DistributionSpaceIterator_SingleGaussian : IDistributionSpaceIterator
	{
		// IEnumerable<IDistribution> Members
		public IEnumerator<IDistribution> GetEnumerator()
		{
			while (hasNext()) {
				yield return next();
			}
		}

		//IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
	        throw new ArgumentException("DistributionSpaceIterator_SingleGaussian does not support System.Collections.IEnumerable.GetEnumerator");  
		}
		
		public bool hasNext() {
			return (_current != null);
		}
		
		public IDistribution next() {
			IDistribution answer = _current;
			if (_ds.MeanIterator.hasNext()) {
				_mean = _ds.MeanIterator.next();
				_current = new Distribution_Gaussian(_ds.BlauSpace, _mean, _std);
			}
			else {
				_ds.MeanIterator.reset();
				_mean = _ds.MeanIterator.next();
				
				if (_ds.StdIterator.hasNext()) {
					_std = _ds.StdIterator.next();
					_current = new Distribution_Gaussian(_ds.BlauSpace, _mean, _std);
				}
				else {
					_current = null;
				}
			}
			return answer;
		}
		
		public void reset() {
			_ds.MeanIterator.reset();
			_ds.StdIterator.reset();
		}
		
		private DistributionSpace_SingleGaussian _ds;
		public IDistributionSpace Space {
			get {return _ds;}
		}
		
		private IBlauPoint _mean;
		private IBlauPoint _std;
		private Distribution_Gaussian _current;
		
		public DistributionSpaceIterator_SingleGaussian (IBlauSpace space, IBlauSpaceIterator meanIter, IBlauSpaceIterator stdIter)
		{
			_ds = new DistributionSpace_SingleGaussian(space, meanIter.clone(), stdIter.clone());
			_std = _ds.StdIterator.next();
			_mean = _ds.MeanIterator.next();
			_current = new Distribution_Gaussian(space, _mean, _std);
		}
		
		public override string ToString ()
		{
			string s = "DistributionSpaceIterator_SingleGaussian @ ("+_current+") in "+_ds+"";
			return s;
		}
	}
}

