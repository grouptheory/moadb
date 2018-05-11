using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace dist
{
	[Serializable]
	public abstract class AbstractDistribution : IDistribution
	{	
		protected IBlauSpace _space;
		public IBlauSpace SampleSpace {
			get {return _space;}
		}
		
		public AbstractDistribution(IBlauSpace space, int par)
		{
			_space = space;
			_params = par;
			_totalSamples = -1;
		}
		
		protected AbstractDistribution(AbstractDistribution orig) {
			_space = orig._space;
			_params = 0;
			_totalSamples = -1;
		}
			
		public abstract bool IsValid();
		public abstract void DistributionComplete();
		
		public Object GetRealObject(StreamingContext context)  {
			return this;
    	}
		
		private int _params;
		public int Params {
			get { return _params; }
		}
		protected void addParams(int n) {
			_params += n;
		}
		
		public abstract IBlauPoint getSample();

		private int _totalSamples = -1;
		public int TotalSamples {
			get { return _totalSamples; }
		}

		private int _sampleNumber = 0;
		public int SampleNumber {
			get { return _sampleNumber; }
		}
		protected void incrementSampleNumber ()
		{
			_sampleNumber++;
		}

		public virtual void setTotalSamples(int totalSamples) {
			if (totalSamples <= 0) throw new Exception("AbstractCompositeDistribution setTotalSamples called with <= 0");

			// Console.WriteLine ("FFF "+totalSamples);

			_totalSamples = totalSamples;
			_sampleNumber = 0;
		}

		public abstract string getParamName(int pn);
		
		public abstract double getParam(int pn);
		public abstract void setParam(int pn, double val);
		
		public abstract double getParamMin(int pn);
		public abstract void setParamMin(int pn, double val);
		
		public abstract double getParamMax(int pn);
		public abstract void setParamMax(int pn, double val);
		
		public abstract IDistribution clone();
		
		public abstract string ToString (int indent);

		protected static double EPSI = 0.00001;
		protected bool IsSignificantlyGreater(double a, double b) {
			return a>b+EPSI;
		}
		protected bool IsSignificantlySmaller(double a, double b) {
			return a<b-EPSI;
		}
	}
}

