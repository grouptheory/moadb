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
	public class Distribution_Pointed_Immutable : AbstractAtomicDistribution 
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
			return true;
		}
		
		private double _value;
		
		public double Value {
			get { return _value; }
		}
		
		public override string getParamName(int pn) {
			throw new Exception ("Distribution_Pointed_Immutable getParamName cannot occur");
		}
		
		public override double getParam(int pn) {
			throw new Exception ("Distribution_Pointed_Immutable getParam cannot occur");
		}
		
		public override void setParam(int pn, double val) {
			throw new Exception ("Distribution_Pointed_Immutable setParam cannot occur");
		}
		
		public Distribution_Pointed_Immutable (IBlauSpace space, double v) : base(space, 0)
		{
			_value = v;
		}
		
		protected Distribution_Pointed_Immutable(Distribution_Pointed_Immutable orig) : base(orig) {
			_value = orig.Value;
		}
		
		public override IDistribution clone() {
			return new Distribution_Pointed_Immutable(this);
		}
		
		public override string ToString ()
		{
			return this.ToString(0);
		}
		
		public override string ToString (int indent)
		{
			string spc = "";
			for (int i=0;i<indent;i++) spc+=" ";
			
			string s = spc+"PointedImmutable["+this.SampleSpace+"]"+ this.Params + "(value="+_value+")\n";
			return s;
		}
		
		[OnDeserialized()]
		internal void register(StreamingContext context)
		{
			SingletonLogger.Instance().DebugLog(typeof(Distribution_Pointed), "PointedImmutable OnDeserialized ...");
			this._space = BlauSpaceRegistry.Instance().validate(this._space);
		}
	}
}

