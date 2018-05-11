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
	public class Mixture_Immutable : AbstractCompositeDistribution
	{
		public Mixture_Immutable(IBlauSpace space) : base(space, 0)
		{
			// _component = new Dictionary<IDistribution, double>();
			// _component = new C5.HashDictionary<IDistribution, double>();
			_component = new List<MixtureComponent>();
			
			_completed = false;
		}
		
		protected Mixture_Immutable(Mixture_Immutable orig) : base(orig) {
			// _component = new Dictionary<IDistribution, double>();
			// _component = new C5.HashDictionary<IDistribution, double>();
			_component = new List<MixtureComponent>();
			
			foreach (MixtureComponent mc in orig._component) {
				IDistribution d = mc.Distribution;
				double w = mc.Weight;
				Add(d.clone(), w);
			}
			_completed = orig._completed;
		}
		
		public override IDistribution clone() {
			return new Mixture_Immutable(this);
		}
		
		public override void setTotalSamples(int totalSamples) {
			int runningTotal = 0;
			
			foreach (MixtureComponent mc in _component) {
				IDistribution d = mc.Distribution;
				double w = mc.Weight;
				int subSamples = (int)(w * (double)totalSamples);
				
				if (mc == _component.ToArray()[_component.Count-1]) {
					subSamples = totalSamples - runningTotal;
				}
				
				d.setTotalSamples(subSamples);
				runningTotal += subSamples;
			}
			base.setTotalSamples(totalSamples);
		}
		
		// private C5.HashDictionary<IDistribution, double> _component;
		// private Dictionary<IDistribution, double> _component;
		private List<MixtureComponent> _component;
		
		protected override List<IDistribution> getComponents() {
			List<IDistribution> converted = new List<IDistribution>();
			foreach (MixtureComponent mc in _component) {
				IDistribution d = mc.Distribution;
				converted.Add(d);
			}
			return converted;
		}
		
		protected override int AdditionalPerComponentParameters {
			get { return 0; }
		}
		
		public override bool IsValid() {
			if ( ! base.IsValid() ) return false;
			if ( _component.Count == 0 ) return false;
			
			if ( IsSignificantlyGreater( totalWeight() , 1.0 )) return false;
			if ( IsSignificantlySmaller( totalWeight() , 1.0 )) return false;
			
			SingletonLogger.Instance().DebugLog(typeof(Mixture), "_component.Count: "+ _component.Count);

			return true;
		}
		
		protected override double getParam_AdditionalPerComponentParameters(IDistribution x, int pn) {
			throw new Exception ("Mixture_Immutable getParam_AdditionalPerComponentParameters cannot occur");
		}
		protected override void setParam_AdditionalPerComponentParameters(IDistribution x, int pn, double val) {
			throw new Exception ("Mixture_Immutable setParam_AdditionalPerComponentParameters cannot occur");
		}
		protected override double getParamMin_AdditionalPerComponentParameters(IDistribution x, int pn) {
			throw new Exception ("Mixture_Immutable getParamMin_AdditionalPerComponentParameters cannot occur");
		}
		protected override void setParamMin_AdditionalPerComponentParameters(IDistribution x, int pn, double val) {
			throw new Exception ("Mixture_Immutable setParamMin_AdditionalPerComponentParameters cannot occur");
		}
		protected override double getParamMax_AdditionalPerComponentParameters(IDistribution x, int pn) {
			throw new Exception ("Mixture_Immutable getParamMax_AdditionalPerComponentParameters cannot occur");
		}
		protected override void setParamMax_AdditionalPerComponentParameters(IDistribution x, int pn, double val) {
			throw new Exception ("Mixture_Immutable setParamMax_AdditionalPerComponentParameters cannot occur");
		}
		
		private bool _completed;
		public override void DistributionComplete() {
			if ( _component.Count == 0 ) {
				throw new Exception("Mixture distribution has no components");
			}
			
			if ( totalWeight() <= 0.0 ) {
				throw new Exception("Mixture distribution has non-positive totalWeight");
			}
			
			foreach (MixtureComponent mc in _component) {
				IDistribution d = mc.Distribution;
				d.DistributionComplete();
			}
			
			normalize();
			_completed = true;
		}
		
		public void Add(IDistribution d, double w) {
			if ( _completed ) {
				throw new Exception("Attempt to add a distribution to a Mixture that is marked completed");
			}
			if (d.SampleSpace != this.SampleSpace) {
				throw new Exception("Incompatible IDistribution added to Mixture");
			}
			
			//if (!_component.ContainsKey(d)) {
			// if (!_component.Contains(d)) {
			if ( ! Contains (d)) {
				MixtureComponent mc = new MixtureComponent();
				mc.Distribution = d;
				mc.Weight = w;
				mc.MinWeight = 0.0;
				mc.MaxWeight = 1.0;
				
				_component.Add(mc);
				
				// the parameters of the component distribution
				this.addParams(d.Params);
				// the weight of the component distribution in NOT a parameter in an immutable mixture
				// this.addParams(AdditionalPerComponentParameters);
			}
			else {
				throw new Exception("Duplicate IDistribution added to Mixture");
			}
		}
		
		private void SetMax(IDistribution d, double maxw) {
			
			if (d.SampleSpace != this.SampleSpace) {
				throw new Exception("Incompatible IDistribution lookup in Mixture");
			}
			
			foreach (MixtureComponent mc in _component) {
				if (mc.Distribution == d) {
					mc.MaxWeight = maxw;
					return;
				}
			}
			
			throw new Exception("lookup unknown IDistribution in Mixture");
		}
		
		private void SetMin(IDistribution d, double minw) {
			
			if (d.SampleSpace != this.SampleSpace) {
				throw new Exception("Incompatible IDistribution lookup in Mixture");
			}
			
			foreach (MixtureComponent mc in _component) {
				if (mc.Distribution == d) {
					mc.MinWeight = minw;
					return;
				}
			}
			
			throw new Exception("lookup unknown IDistribution in Mixture");
		}
		
		private void Set(IDistribution d, double w) {
			
			if (d.SampleSpace != this.SampleSpace) {
				throw new Exception("Incompatible IDistribution lookup in Mixture");
			}
			
			foreach (MixtureComponent mc in _component) {
				if (mc.Distribution == d) {
					mc.Weight = w;
					return;
				}
			}
			
			throw new Exception("lookup unknown IDistribution in Mixture");
		}
		
		private double LookupMax(IDistribution d) {
			foreach (MixtureComponent mc in _component) {
				IDistribution dx = mc.Distribution;
				double w = mc.MaxWeight;
				if (d==dx) return w;
			}
			throw new Exception("lookup unknown IDistribution in Mixture");
		}
		
		
		private double LookupMin(IDistribution d) {
			foreach (MixtureComponent mc in _component) {
				IDistribution dx = mc.Distribution;
				double w = mc.MinWeight;
				if (d==dx) return w;
			}
			throw new Exception("lookup unknown IDistribution in Mixture");
		}
		
		private double Lookup(IDistribution d) {
			foreach (MixtureComponent mc in _component) {
				IDistribution dx = mc.Distribution;
				double w = mc.Weight;
				if (d==dx) return w;
			}
			throw new Exception("lookup unknown IDistribution in Mixture");
		}
		
		private bool Contains(IDistribution d) {
			foreach (MixtureComponent mc in _component) {
				IDistribution dx = mc.Distribution;
				if (d==dx) return true;
			}
			return false;
		}
		
		private double totalWeight() {
			double total = 0.0;
			
			SingletonLogger.Instance().DebugLog(typeof(Mixture), "_component.Keys.Count: "+_component.Count);
			
			foreach (MixtureComponent mc in _component) {
				double w = mc.Weight;
				total += w;
			}
			return total;
		}
		
		private void normalize() {
			double total = totalWeight();
			
			// Dictionary<IDistribution, double> component2 = new Dictionary<IDistribution, double>();
			// C5.HashDictionary<IDistribution, double> component2 = new C5.HashDictionary<IDistribution, double>();
			List<MixtureComponent> component2 = new List<MixtureComponent>();
			
			foreach (MixtureComponent mc in _component) {
				IDistribution d = mc.Distribution;
				double w = mc.Weight;
				
				MixtureComponent mc2 = new MixtureComponent();
				mc2.Distribution = d;
				mc2.Weight = w/total;
				
				component2.Add(mc2);
			}
			
			_component = component2;
		}
		
		private static bool DETERMINISTIC_MODE = true;
		
		public override IBlauPoint getSample ()
		{
			if (DETERMINISTIC_MODE) {
				IBlauPoint p = getSample(SampleNumber, TotalSamples);
				incrementSampleNumber();
				return p;
			}
			
			if ( !_completed ) {
				throw new Exception("Attempt to sample from Mixture distribution that has not been completed");
			}
			
			IDistribution d = null;
			double val = SingletonRandomGenerator.Instance.NextDouble();
			
			SingletonLogger.Instance().DebugLog(typeof(Mixture), "toss: "+val);
			
			double total = 0.0;
			foreach (MixtureComponent mc in _component) {
				IDistribution dx = mc.Distribution;
				double w = mc.Weight;
				
				total += w;
				if (total > val) {
					d = dx;
					break;
				}
			}
			
			return d.getSample();
		}
		
		public IBlauPoint getSample (int sampleNum, int totalSamples)
		{
			if (!_completed) {
				throw new Exception ("Attempt to sample from Mixture distribution that has not been completed");
			}
			
			if (TotalSamples<0 || totalSamples<0) {
				throw new Exception ("Deterministic mixture called without declaring total number of samples");
			}
			if (TotalSamples != totalSamples) {
				throw new Exception ("Deterministic mixture called without incoherent total number of samples");
			}
			if (sampleNum > totalSamples) {
				throw new Exception ("Deterministic mixture called too many times -- exceeded total number of samples = "+totalSamples);
			}
			
			IDistribution d = null;
			double val = (double)sampleNum/(double)(totalSamples - 1.0);
			
			SingletonLogger.Instance().DebugLog(typeof(Mixture), "toss: "+val);
			
			double total = 0.0;
			foreach (MixtureComponent mc in _component) {
				IDistribution dx = mc.Distribution;
				double w = mc.Weight;
				
				total += w;
				if (total >= val) {
					d = dx;
					break;
				}
			}
			
			// Console.WriteLine ("FFF sampleNum "+sampleNum);
			// Console.WriteLine ("FFF totalSamples "+totalSamples);
			
			return d.getSample();
		}
		
		public override string ToString ()
		{
			return this.ToString(0);
		}
		
		public override string ToString (int indent)
		{
			string spc = "";
			for (int i=0;i<indent;i++) spc+=" ";
			
			string s = spc+"MixtureImmutable["+this.SampleSpace+"]"+ this.Params + "\n";
			s += spc+"(\n";
			foreach (MixtureComponent mc in _component) {
				IDistribution d = mc.Distribution;
				double w = mc.Weight;
				s += spc + "     " + w + " * " + "\n";
				s += d.ToString(indent+5);
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

