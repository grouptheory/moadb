using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;
using blau;
using logger;

namespace dist
{	
	[Serializable]
	public class Distribution_Gaussian : AbstractAtomicDistribution 
	{
		public override IBlauPoint getSample() {
			BlauPoint p = new BlauPoint(this.SampleSpace);
			for (int i=0; i<this.SampleSpace.Dimension; i++) {
				double val;
				do {
					val = SingletonRandomGenerator.Instance.NextGaussian(_mean, _std);
				}
				while ((val < SampleSpace.getAxis(i).MinimumValue) || (val > SampleSpace.getAxis(i).MaximumValue));
				p.setCoordinate(i, val);
			}
			return p;
		}
		
		public override bool IsValid() {
			if ( ! base.IsValid()) return false;
			if (IsSignificantlySmaller(Mean , this.getParamMin(0))) return false;
			if (IsSignificantlyGreater(Mean , this.getParamMax(0))) return false;
			if (IsSignificantlySmaller(Std , this.getParamMin(1))) return false;
			if (IsSignificantlyGreater(Std , this.getParamMax(1))) return false;
			return true;
		}
		
		private double _mean;
		private double _std;
		
		public double Mean {
			get { return _mean; }
		}
		
		public double Std {
			get { return _std; }
		}
		
		public override string getParamName(int pn) {
			string axis = this.SampleSpace.getAxis(0).Name;
			if (pn==0) return axis+"-Mean";
			else if (pn==1) return axis+"-Std";
			else throw new Exception ("Distribution_Gaussian getParamName index out of bounds "+pn);
		}
		
		public override double getParam(int pn) {
			if (pn==0) return Mean;
			else if (pn==1) return Std;
			else throw new Exception ("Distribution_Gaussian getParam index out of bounds "+pn);
		}
		
		public override void setParam(int pn, double val) {
			if (pn==0) {
				//SingletonLogger.Instance().DebugLog(typeof(Distribution_Gaussian), "Set Gaussian mean ...");
				_mean = val;
			}
			else if (pn==1) {
				//SingletonLogger.Instance().DebugLog(typeof(Distribution_Gaussian), "Set Gaussian std ...");
				_std = val;
			}
			else throw new Exception ("Distribution_Gaussian setParam index out of bounds "+pn);
		}
		
		public Distribution_Gaussian (IBlauSpace space, double mean, double std) : base(space, 2)
		{
			_mean = mean;
			_std = std;
		}
		
		protected Distribution_Gaussian(Distribution_Gaussian orig) : base(orig) {
			_mean = orig._mean;
			_std = orig._std;
			this.addParams(2);
		}
		
		public override IDistribution clone() {
			return new Distribution_Gaussian(this);
		}
		
		public override string ToString ()
		{
			return this.ToString(0);
		}
		
		public override string ToString (int indent)
		{
			string spc = "";
			for (int i=0;i<indent;i++) spc+=" ";
			
			string s = spc+"Gaussian["+this.SampleSpace+"]"+ this.Params + "(mean="+_mean+", "+"std="+_std+")\n";
			return s;
		}
		
	    [OnDeserialized()]
	    internal void register(StreamingContext context)
	    {
			SingletonLogger.Instance().DebugLog(typeof(Distribution_Gaussian), "Gaussian OnDeserialized ...");
			this._space = BlauSpaceRegistry.Instance().validate(this._space);
	    }
	}
}

