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
	public class Distribution_Interval : AbstractAtomicDistribution 
	{
		public override IBlauPoint getSample() {
			BlauPoint p = new BlauPoint(this.SampleSpace);
			for (int i=0; i<this.SampleSpace.Dimension; i++) {
				double val;
				do {
					val = SingletonRandomGenerator.Instance.NextDouble() * (_max - _min) + _min;
				}
				while ((val < SampleSpace.getAxis(i).MinimumValue) || (val > SampleSpace.getAxis(i).MaximumValue));
				p.setCoordinate(i, val);
			}
			return p;
		}

		public override bool IsValid() {
			if ( ! base.IsValid()) return false;
			if (IsSignificantlySmaller(Min , this.getParamMin(0))) return false;
			if (IsSignificantlyGreater(Min , this.getParamMax(0))) return false;
			if (IsSignificantlySmaller(Max , this.getParamMin(1))) return false;
			if (IsSignificantlyGreater(Max , this.getParamMax(1))) return false;
			if (IsSignificantlyGreater(Min , Max)) return false;
			if ( ! IsSignificantlyGreater(Max , Min)) return false;
			return true;
		}
		
		private double _min;
		private double _max;
		
		public double Min {
			get { return _min; }
		}
		
		public double Max {
			get { return _max; }
		}
		
		public override string getParamName(int pn) {
			string axis = this.SampleSpace.getAxis(0).Name;
			if (pn==0) return axis+"-Min";
			else if (pn==1) return axis+"-Max";
			else throw new Exception ("Distribution_Interval getParamName index out of bounds "+pn);
		}
		
		public override double getParam(int pn) {
			if (pn==0) return Min;
			else if (pn==1) return Max;
			else throw new Exception ("Distribution_Interval getParam index out of bounds "+pn);
		}
		
		public override void setParam(int pn, double val) {
			if (pn==0) _min = val;
			else if (pn==1) _max = val;
			else throw new Exception ("Distribution_Interval setParam index out of bounds "+pn);
		}
		
		public Distribution_Interval (IBlauSpace space, double min, double max) : base(space, 2)
		{
			_min = min;
			_max = max;
		}
		
		protected Distribution_Interval(Distribution_Interval orig) : base(orig) {
			_min = orig._min;
			_max = orig._max;
			this.addParams(2);
		}
		
		public override IDistribution clone() {
			return new Distribution_Interval(this);
		}
		
		public override string ToString ()
		{
			return this.ToString(0);
		}
		
		public override string ToString (int indent)
		{
			string spc = "";
			for (int i=0;i<indent;i++) spc+=" ";
			
			string s = spc+"Interval["+this.SampleSpace+"]"+ this.Params + "(min="+_min+", "+"max="+_max+")\n";
			return s;
		}
		
	    [OnDeserialized()]
	    internal void register(StreamingContext context)
	    {
			SingletonLogger.Instance().DebugLog(typeof(Distribution_Interval), "Interval OnDeserialized ...");
			this._space = BlauSpaceRegistry.Instance().validate(this._space);
	    }
	}
}

