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
	public class Distribution_Sequential : AbstractAtomicDistribution
	{
		public override IBlauPoint getSample() {
			IBlauPoint answer = _current;
			BlauPoint p = new BlauPoint(this.SampleSpace);
			for (int i=0; i<this.SampleSpace.Dimension; i++) {
				p.setCoordinate(i, _current.getCoordinate(i));
			}
			for (int i=0; i<this.SampleSpace.Dimension; i++) {
				if (p.getCoordinate(i) + _step > this.SampleSpace.getAxis(i).MaximumValue) {
					p.setCoordinate(i, this.SampleSpace.getAxis(i).MinimumValue);
					if (i==this.SampleSpace.Dimension) {
						p=new BlauPoint(this.SampleSpace);
						break;
					}
				}
				else {
					p.setCoordinate(i, _step + p.getCoordinate(i));
					break;
				}
			}
			_current = p;
			
			return answer;
		}

		public override bool IsValid() {
			if ( ! base.IsValid()) return false;
			if (_step <= 0.0) return false;
			return true;
		}
		
		[NonSerialized()] 
		private IBlauPoint _current;
		
		private double _step;
		
		public double Step {
			get { return _step; }
		}
		
		public override string getParamName(int pn) {
			throw new Exception ("Distribution_Sequential getParamName index out of bounds");
		}
		
		public override double getParam(int pn) {
			throw new Exception ("Distribution_Sequential getParam index out of bounds");
		}
		
		public override void setParam(int pn, double val) {
			throw new Exception ("Distribution_Sequential setParam index out of bounds");
		}
		
		public Distribution_Sequential (IBlauSpace space, double step) : base(space, 0)
		{
			_step = step;
			_current = new BlauPoint(space);
		}
		
		protected Distribution_Sequential(Distribution_Sequential orig) : base(orig) {
			_step = orig._step;
			_current = new BlauPoint(orig.SampleSpace);
			this.addParams(0);
		}
		
		public override IDistribution clone() {
			return new Distribution_Sequential(this);
		}
		
		public override string ToString ()
		{
			return this.ToString(0);
		}
		
		public override string ToString (int indent)
		{
			string spc = "";
			for (int i=0;i<indent;i++) spc+=" ";
			
			string s = spc+"Sequential["+this.SampleSpace+"]"+ this.Params + "(step:"+_step+")\n";
			return s;
		}
		
	    [OnDeserialized()]
	    internal void register(StreamingContext context)
	    {
			SingletonLogger.Instance().DebugLog(typeof(Distribution_Sequential), "Sequential OnDeserialized ...");
			this._space = BlauSpaceRegistry.Instance().validate(this._space);
			_current = new BlauPoint(this.SampleSpace);
	    }
	}
}

