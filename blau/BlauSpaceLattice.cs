using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;
using logger;

namespace blau
{
	/// <summary>
    /// BlauSpaceLattice class
    /// </summary>
	[Serializable]
	public class BlauSpaceLattice : IBlauSpaceLattice, IObjectReference 
	{
		// the hashed name of this lattice, used in the registry to prevent multiple instances on deserialization
		private string _hashedName;
		public string HashedName {
			get {return _hashedName;}
		}
		
		// the blauspace to which this lattice may be applied
		private IBlauSpace _space;
		public IBlauSpace BlauSpace {
			get {return _space;}
		}
		
		// array of gridding numbers
		protected int [] _steps;
		
		// get the gridding on a particular axis
		public int getSteps(int dim) {
			return _steps[dim];
		}
		
		// get the step size in a particular axis
		public double getStepSize(int i) {
			return (this.BlauSpace.getAxis(i).MaximumValue - this.BlauSpace.getAxis(i).MinimumValue) /
				   (double)this.getSteps(i);
		}
		
		// set the gridding on a particular axis
		protected void setSteps(int dim, int val) {
			if (val <= 0) {
				throw new Exception("BlauSpaceLattice setSteps called on dimension "+dim+" with steps < 0");
			}
			_steps[dim] = val;
		}
		
		// quantize a blaupoint with respect to this lattice
		public IBlauPoint quantize(IBlauPoint p) {
			if (p.Space != this.BlauSpace) {
				throw new Exception("BlauSpaceLattice is being requested to quantize a BlauPoint which lies in a foreign BlauSpace");
			}
			
			IBlauPoint qp = new QuantizedBlauPoint(p.Space, this);
			
			for (int i=0; i<p.Space.Dimension; i++) {
				double px = p.getCoordinate(i);
				// qp will quantize coordinates to this lattice
				qp.setCoordinate(i, px);
			}
			return qp;
		}
		
		// private constructor
		private BlauSpaceLattice (IBlauSpace space, int[] steps) : this(space, steps, true)
		{
		}
		
		// protected constructor
		protected BlauSpaceLattice (IBlauSpace space, int[] steps, bool initHashedName)
		{
			if (space==null) {
				throw new Exception("BlauSpaceLattice instantiated with null BlauSpace");
			}
			_space = space;
			
			if (steps.Length != _space.Dimension) {
				throw new Exception("BlauSpaceLattice instantiated with steps dimension != BlauSpace dimension");
			}
			
			_steps = new int [space.Dimension];
			for (int i=0; i<space.Dimension; i++) {
				this.setSteps(i, steps[i]);
			}
			
			if (initHashedName) {
				updateHashedName();
			}
		}
		
		// public static factory method
		public static IBlauSpaceLattice create(IBlauSpace space, int[] steps) {
			BlauSpaceLattice s = new BlauSpaceLattice(space, steps);
			BlauSpaceLatticeRegistry.Instance().add(s);
			BlauSpaceLattice s_validated = BlauSpaceLatticeRegistry.Instance().validate(s);
			return s_validated;
		}
		
		// hashed name, used in the blauspace registry
		protected void updateHashedName() {
			_hashedName = this.ToString ();
		}
		
		// convert this blauspace lattice to a string
		public override string ToString ()
		{
			string s = "";
			s += ("Lattice[ "+BlauSpace+" ]:");
			for (int i=0; i<BlauSpace.Dimension; i++) {
				s+=(""+getSteps(i));
				if (i!=BlauSpace.Dimension-1) s+=" x ";
			}
			return s;
		}
		
		// proxy reference
		public Object GetRealObject(StreamingContext context)  {
			BlauSpaceLattice s_validated = BlauSpaceLatticeRegistry.Instance().validate(this);
			return s_validated;
    	}
		
		// deserializer
	    [OnDeserialized()]
	    internal void register(StreamingContext context)
	    {
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(BlauSpaceLattice), "BSL OnDeserialized ...");
			BlauSpaceLatticeRegistry.Instance().add(this);
	    }
	}
}

