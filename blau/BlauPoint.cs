using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace blau
{
	/// <summary>
    /// BlauPoint class
    /// </summary>
	[Serializable]
	public class BlauPoint : IBlauPoint
	{
		// ambient blauspace
		private IBlauSpace _space = null;
		public IBlauSpace Space {
			get {return _space;}
		}
		
		// coordinate array
		private double [] _coords;
		
		public double getCoordinate(int i) {
			return _coords[i];
		}
		
		// mutability -- once agent blaupoint coordinates are set they are immutable
		[NonSerialized()] 
		private bool _immutable = false;
		
		public void setImmutable() {
			_immutable = true;
		}
		
		private static double EPSILON = 0.00001;
		
		// setters for coordinates
		public virtual void setCoordinate(int i, double val) {
			if (_immutable) {
				throw new Exception("Attempt to alter an immutable BlauPoint");    
			}
			if (val < Space.getAxis(i).MinimumValue-EPSILON) {
				val = Space.getAxis(i).MinimumValue;
				throw new Exception("Attempt to set BlauPoint coordinate to "+val+" outside of BlauSpace range minimum "+Space.getAxis(i).MinimumValue+" on axis "+i);   
			}
			if (val > Space.getAxis(i).MaximumValue+EPSILON) {
				val = Space.getAxis(i).MaximumValue;
				throw new Exception("Attempt to set BlauPoint coordinate to "+val+" outside of BlauSpace range maximum "+Space.getAxis(i).MaximumValue+" on axis "+i);   
			}
			_coords[i] = val;
			_dirty = true;
		}
		
		// copy of this blaupoint
		public virtual IBlauPoint clone() {
			IBlauPoint dupe = new BlauPoint(this.Space);
			for (int i=0; i<this.Space.Dimension; i++) {
				dupe.setCoordinate(i, this.getCoordinate(i));
			}
			return dupe;
		}
		
		// comparator
		public virtual int CompareTo(object obj) {
	        if(obj is BlauPoint) {
	            BlauPoint p = (BlauPoint)obj;
				for (int i=0; i<this.Space.Dimension; i++) {
					if (this.getCoordinate(i) < p.getCoordinate(i)) return -1;
					if (this.getCoordinate(i) > p.getCoordinate(i)) return +1;
				}
				return 0;
	        }
	        throw new ArgumentException("object is not a BlauPoint");    
	    }
		
		// hashcode permits use in dictionary keys
		[NonSerialized()] 
		private int _hashcode = 0;
		
		// lazy recalculation flag
		[NonSerialized()] 
		private bool _dirty = true;
		
		// recalculate hash code
		protected virtual int RecalculateHashCode() {
	        double val = 0.0;
			for (int i=0; i<this.Space.Dimension; i++) {
				val += 1.0 / getCoordinate(i);
			}
			double valint = (int) val;
			val -= (double)valint;
			return (int)(val * (double)Int32.MaxValue);
		}
		
		// get hash code
	    public override int GetHashCode()
	    {
			if (_dirty) {
				_hashcode = RecalculateHashCode();
				_dirty = false;
			}
			return _hashcode;
	    }
		
		// constructor
		public BlauPoint(IBlauSpace space)
		{
			_space = space;
			_coords = new double [space.Dimension];
			for (int i=0; i<space.Dimension; i++) {
				_coords[i] = space.getAxis(i).MinimumValue;
			}
			_dirty = true;
		}
		
		// copy constructor
		public BlauPoint(IBlauPoint orig)
		{
			_space = orig.Space;
			_coords = new double [orig.Space.Dimension];
			for (int i=0; i<orig.Space.Dimension; i++) {
				_coords[i] = orig.getCoordinate(i);
			}
			_dirty = true;
		}
		
		// diagnostic to string
		public override string ToString ()
		{
			string s = "";
			for (int i=0; i<Space.Dimension; i++) {
				s+=(""+getCoordinate(i));
				if (i!=Space.Dimension-1) s+="\t";
			}
			return s;
		}
	}
}

