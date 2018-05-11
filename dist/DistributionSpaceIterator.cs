using System;
using System.Collections.Generic;
using core;
using blau;
using logger;

namespace dist
{
	public class DistributionSpaceIterator : IDistributionSpaceIterator
	{
		private IDistributionSpace _ds;
		private IDistribution _templateDistribution;
		private IBlauSpaceIterator _parmSpaceIterator;
		
		public IDistributionSpace Space {
			get {return _ds;}
		}
		
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
	        throw new ArgumentException("DistributionSpaceIterator does not support System.Collections.IEnumerable.GetEnumerator");  
		}
		
		public bool hasNext() {
			//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "DistributionSpaceIterator.hasNext: "+(_templateDistribution != null));
			return (_templateDistribution != null);
		}
		
		public IDistribution next() {
			//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "pre templateDistribution: "+_templateDistribution);
			IDistribution answer = _templateDistribution.clone();
			//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "post templateDistribution: "+_templateDistribution);
			//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "pre answer: "+answer);
			advance();
			//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "post answer: "+answer);
			return answer;
		}

		private IBlauPoint _parms;

		private void advance() {
			if (_parmSpaceIterator.hasNext()) {
				//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "_parmSpaceIterator.hasNext: true");
				_parms = _parmSpaceIterator.next();
				//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "_parmSpaceIterator.next: "+parms);
				FillTemplateDistributionParameters(_parms);
				//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "_templateDistribution: "+_templateDistribution);
			}
			else {
				//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "_parmSpaceIterator.hasNext: false");
				_templateDistribution = null;
				//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "_templateDistribution: null");
			}
		}
		
		public void reset() {
			_parmSpaceIterator.reset();
		}
		
		public DistributionSpaceIterator (IDistributionSpace ds, int[] steps)
		{
			_ds =ds;
			// _parmSpaceIterator = new BlauSpaceIterator(ds.ParamSpace, steps);
			_parmSpaceIterator = new BlauSpaceIterator(ds.ParamSpace, steps);
			
			_templateDistribution = ds.TemplateDistribution.clone();
			advance();
			SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "inital _templateDistribution: "+_templateDistribution);
		}
		
		private void FillTemplateDistributionParameters(IBlauPoint parms) {
			for (int i=0; i<parms.Space.Dimension; i++) {
				//SingletonLogger.Instance().DebugLog(typeof(DistributionSpaceIterator), "now filling param "+i+" with "+parms.getCoordinate(i));
				_templateDistribution.setParam(i, parms.getCoordinate(i));

				//Console.WriteLine("Now filling param "+i+" with "+parms.getCoordinate(i));
			}
			
			//Console.WriteLine("the template dist is now: "+_templateDistribution);
		}
		
		public override string ToString ()
		{
			string s = (_parms != null ? _parms.ToString() : "FROBBY");
			return s;
		}
	}
}

