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
	public class Distribution_Pointed : AbstractAtomicDistribution 
	{
		public override IBlauPoint getSample() {
			BlauPoint p = new BlauPoint(this.SampleSpace);
			for (int i=0; i<this.SampleSpace.Dimension; i++) {
				double val = Value;
				p.setCoordinate(i, val);
			}
			return p;
		}

		public override bool IsValid ()
		{
			if (! base.IsValid ()) {
				return false;
			}
			if (IsSignificantlySmaller(Value , this.getParamMin (0))) {
				Console.WriteLine("Invalid 4: param"+" "+Value+" < "+this.getParamMin (0)+" in "+this);
				return false;
			}
			if (IsSignificantlyGreater(Value , this.getParamMax (0))) {
				Console.WriteLine("Invalid 5: param"+" "+Value+" > "+this.getParamMax (0)+" in "+this);
				return false;
			}
			return true;
		}
		
		private double _value;
		
		public double Value {
			get { return _value; }
		}
		
		public override string getParamName(int pn) {
			string axis = this.SampleSpace.getAxis(0).Name;
			if (pn==0) return axis+"-Value";
			else throw new Exception ("Distribution_Pointed getParamName index out of bounds "+pn);
		}
		
		public override double getParam(int pn) {
			if (pn==0) return Value;
			else throw new Exception ("Distribution_Pointed getParam index out of bounds "+pn);
		}
		
		public override void setParam(int pn, double val) {
			if (pn==0) _value = val;
			else throw new Exception ("Distribution_Pointed setParam index out of bounds "+pn);
		}
		
		public Distribution_Pointed (IBlauSpace space, double v) : base(space, 1)
		{
			_value = v;
		}
		
		protected Distribution_Pointed(Distribution_Pointed orig) : base(orig) {
			_value = orig.Value;
			this.addParams(1);
		}
		
		public override IDistribution clone() {
			return new Distribution_Pointed(this);
		}
		
		public override string ToString ()
		{
			return this.ToString(0);
		}
		
		public override string ToString (int indent)
		{
			string spc = "";
			for (int i=0;i<indent;i++) spc+=" ";
			
			string s = spc+"Pointed["+this.SampleSpace+"]"+ this.Params + "(value="+_value+")\n";
			return s;
		}
		
	    [OnDeserialized()]
	    internal void register(StreamingContext context)
	    {
			SingletonLogger.Instance().DebugLog(typeof(Distribution_Pointed), "Pointed OnDeserialized ...");
			this._space = BlauSpaceRegistry.Instance().validate(this._space);
	    }
	}
}

