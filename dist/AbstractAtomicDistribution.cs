using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace dist
{
	[Serializable]
	public abstract class AbstractAtomicDistribution : AbstractDistribution
	{
		public AbstractAtomicDistribution(IBlauSpace space, int par) : base(space, par)
		{
			if (space.Dimension != 1) {	
				throw new Exception ("AbstractAtomicDistribution must be one-dimensional");
			}
			
			initParamRange(par);
		}
		
		protected AbstractAtomicDistribution(AbstractAtomicDistribution orig) : base(orig) {
			_paramMin = new double [orig.Params];
			_paramMax = new double [orig.Params];
			for (int i=0; i<orig.Params; i++) {
				_paramMin[i] = orig._paramMin[i];
				_paramMax[i] = orig._paramMax[i];
			}
		}

		public override bool IsValid() {
			for (int i=0; i<Params; i++) {
				double pi = getParam(i);
				if (IsSignificantlySmaller(_paramMax[i] , _paramMin[i])) {
					Console.WriteLine("Invalid 1: param"+i+" "+_paramMax[i]+" < "+_paramMin[i]+" in "+this);
					return false;
				}
				if (IsSignificantlySmaller(pi , _paramMin[i]))  {
					Console.WriteLine("Invalid 2: param"+i+" "+getParam(i)+" < "+_paramMin[i]+" in "+this);
					return false;
				}
				if (IsSignificantlyGreater(pi , _paramMax[i]))  {
					Console.WriteLine("Invalid 3: param"+i+" "+getParam(i)+" > "+_paramMax[i]+" in "+this);
					return false;
				}
			}
			return true;
		}
		
		public override void DistributionComplete() {
		}
		
		private double [] _paramMin;
		private double [] _paramMax;
		
		private void initParamRange(int par) {
			_paramMin = new double [par];
			_paramMax = new double [par];
			for (int i=0; i<Params; i++) {
				_paramMin[i] = this.SampleSpace.getAxis(0).MinimumValue;
				_paramMax[i] = this.SampleSpace.getAxis(0).MaximumValue;
			}
		}
		
		public override double getParamMin(int pn) {
			if ((pn < 0) || (pn >= Params)) throw new Exception("AbstractAtomicDistribution getParamMin index out of range!");
			return _paramMin[pn];
		}
		
		public override void setParamMin(int pn, double val) {
			if ((pn < 0) || (pn >= Params)) throw new Exception("AbstractAtomicDistribution setParamMin index out of range!");
			_paramMin[pn] = val;
		}
		
		public override double getParamMax(int pn) {
			if ((pn < 0) || (pn >= Params)) throw new Exception("AbstractAtomicDistribution getParamMax index out of range!");
			return _paramMax[pn];
		}
		
		public override void setParamMax(int pn, double val) {
			if ((pn < 0) || (pn >= Params)) throw new Exception("AbstractAtomicDistribution setParamMax index out of range!");
			_paramMax[pn] = val;
		}
	}
}

