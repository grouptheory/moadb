using System;
using System.Collections.Generic;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;
using blau;
using logger;

namespace dist
{
	[Serializable]
	public class Product : AbstractCompositeDistribution
	{
		public Product(IBlauSpace space) : base(space, 0)
		{
			_factor = new List<IDistribution>();
			_completed = false;
		}
		
		protected Product(Product orig) : base(orig) {
			_factor = new List<IDistribution>();
			foreach (IDistribution d in orig._factor) {
				Add(d.clone());
			}
			_completed = orig._completed;
		}
		
		public override IDistribution clone() {
			return new Product(this);
		}
		
		private List<IDistribution> _factor;
		
		protected override List<IDistribution> getComponents() {
			List<IDistribution> converted = new List<IDistribution>();
			foreach (IDistribution x in _factor) converted.Add(x);
			return converted;
		}
		
		protected override int AdditionalPerComponentParameters {
			get { return 0; }
		}
		
		public override bool IsValid() {
			if ( ! base.IsValid() ) return false;
			if ( !IsCovered() )  return false;
			return true;
		}
		
		protected override double getParam_AdditionalPerComponentParameters(IDistribution x, int pn) {
			throw new Exception("Product getParam_AdditionalPerComponentParameters index out of range!");
		}
		protected override void setParam_AdditionalPerComponentParameters(IDistribution x, int pn, double val) {
			throw new Exception("Product setParam_AdditionalPerComponentParameters index out of range!");
		}
		protected override double getParamMin_AdditionalPerComponentParameters(IDistribution x, int pn) {
			throw new Exception("Product getParamMin_AdditionalPerComponentParameters index out of range!");
		}
		protected override void setParamMin_AdditionalPerComponentParameters(IDistribution x, int pn, double val) {
			throw new Exception("Product setParamMin_AdditionalPerComponentParameters index out of range!");
		}
		protected override double getParamMax_AdditionalPerComponentParameters(IDistribution x, int pn) {
			throw new Exception("Product getParamMax_AdditionalPerComponentParameters index out of range!");
		}
		protected override void setParamMax_AdditionalPerComponentParameters(IDistribution x, int pn, double val) {
			throw new Exception("Product setParamMax_AdditionalPerComponentParameters index out of range!");
		}
		
		private bool _completed;
		public override void DistributionComplete() {
			if ( !IsCovered() ) {
				throw new Exception("Product distribution BlauSpace is not covered by its constituent factors");
			}
			
			foreach (IDistribution d in _factor) {
				d.DistributionComplete();
			}
			
			_completed = true;
		}
		
		public void Add(IDistribution d) {
			if ( _completed ) {
				throw new Exception("Attempt to add a distribution to a Product that is marked completed");
			}
			if ( ! IsWithinProductBlauSpace(d.SampleSpace)) {
				Console.Out.WriteLine("  small space: "+d.SampleSpace);
				Console.Out.WriteLine("  big space: "+this.SampleSpace);
				throw new Exception("Incompatible distribution added to product, not contained in ambient space");
			}
			if ( IntersectsFactors(d.SampleSpace)) {
				throw new Exception("Incompatible distribution added to product, ambiguity due to axis duplication");
			}
			
			_factor.Add(d);
			
			// the parameters of the component distribution
			this.addParams(d.Params);
			// nothing additional
			this.addParams(AdditionalPerComponentParameters);
		}
		
		private bool IsWithinProductBlauSpace(IBlauSpace small) {
			return BlauSpace.contains(this.SampleSpace, small);
		}
		
		private bool IntersectsFactors(IBlauSpace s1) {
			foreach (IDistribution d in _factor) {
				if (BlauSpace.intersects(s1, d.SampleSpace)) return true;
			}
			return false;
		}
		
		private bool IsCovered() {
			for (int i=0; i<this.SampleSpace.Dimension;i++) {
				bool covered = false;
				foreach (IDistribution d in _factor) {
					if (d.SampleSpace.hasAxis(this.SampleSpace.getAxis(i).Name)) {
						covered = true;
						break;
					}
				}
				if ( ! covered) return false;
			}
			return true;
		}
		
		public override IBlauPoint getSample() {
			if ( !_completed ) {
				throw new Exception("Attempt to sample from Product distribution that has not been completed");
			}
			IBlauPoint p = new BlauPoint(this.SampleSpace);
			foreach (IDistribution d in _factor) {
				IBlauPoint q = d.getSample();
				for (int i=0;i<q.Space.Dimension; i++) {
					p.setCoordinate( p.Space.getAxisIndex(q.Space.getAxis(i).Name), q.getCoordinate(i) );
				}
			}
			return p;
		}

		public override string ToString ()
		{
			return this.ToString(0);
		}
		
		public override string ToString (int indent)
		{
			string spc = "";
			for (int i=0;i<indent;i++) spc+=" ";
			
			string s = spc+"Product["+this.SampleSpace+"] "+ this.Params + "\n";
			s += spc+"(\n";
			
			int count = 0;
			foreach (IDistribution d in _factor) {
				count++;
				s += d.ToString(indent+5);
				if (count < _factor.Count) {
					s += spc+"     "+"CROSS\n";
				}
			}
			s += spc+")\n";
			return s;
		}
		
		[OnDeserialized()]
	    internal void register(StreamingContext context)
	    {
			this._space = BlauSpaceRegistry.Instance().validate(this._space);
	    }
	}
}

