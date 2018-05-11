using System;
using System.Collections.Generic;
using core;

namespace blau
{
	/// <summary>
    /// BlauSpaceIterator class
    /// </summary>
	public class BlauSpaceIterator : BlauSpaceLattice, IBlauSpaceIterator
	{
		// integral lattice coordinates
		private int [] _coords;
		// iteration process is complete?
		private bool _done;
		
		// get coordinates of current point -- these are integral lattice coordinates
		private int getCoordinate(int dim) {
			return _coords[dim];
		}
		
		// set coordinates of current point -- these are integral lattice coordinates
		private void setCoordinate(int dim, int val) {
			_coords[dim] = val;
		}
		
		// reset current coordinates -- these are integral lattice coordinates
		public void reset() {
			for (int i=0; i<this.BlauSpace.Dimension; i++) {
				this.setCoordinate(i, 0);
			}
			_done = false;
		}
		
		// IEnumerable<IBlauPoint> Members
		public IEnumerator<IBlauPoint> GetEnumerator()
		{
			while (hasNext()) {
				yield return next();
			}
		}

		//IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
	        throw new ArgumentException("BlauSpaceIterator does not support System.Collections.IEnumerable.GetEnumerator");  
		}
		
		// are there more BlauSpace points?
		public bool hasNext() {
			return !_done;
		}
		
		// convert integral lattice coordinates to real blauspace coordinates
		private IBlauPoint getCurrentBlauPoint() {
			BlauPoint p = new QuantizedBlauPoint(this.BlauSpace, this);
			for (int i=0; i<this.BlauSpace.Dimension; i++) {
				double coord = 
					this.BlauSpace.getAxis(i).MinimumValue + 
					((double)this.getCoordinate(i) * getStepSize(i));
				
				p.setCoordinate(i, coord);      
			}
			return p;
		}
		
		// retrieve next BlauSpace point
		public IBlauPoint next() {
			
			IBlauPoint p = getCurrentBlauPoint();
			
			if (this.BlauSpace.Dimension > 0) {
				int dim = this.BlauSpace.Dimension-1;
				bool finishedIncrement = false;
				
				while (!finishedIncrement) {
					this.setCoordinate(dim, this.getCoordinate(dim) + 1);
					if (this.getCoordinate(dim) == this.getSteps(dim)+1) {
						this.setCoordinate(dim, 0);
						dim--;
						if (dim < 0) {
							_done = true;
							finishedIncrement = true;
						}
					}
					else {
						finishedIncrement = true;
					}
				}
			}
			else {
				_done = true;
			}
			
			return p;
		}
		
		// public constructor
		public BlauSpaceIterator (IBlauSpace space, int[] steps) : base(space, steps, false)
		{
			_coords = new int [space.Dimension];
			reset();
			updateHashedName();
		}
		
		// copy constructor
		private BlauSpaceIterator (BlauSpaceIterator orig) : base(orig.BlauSpace, orig._steps, false)
		{
			_coords = new int [orig.BlauSpace.Dimension];
			
			for (int i=0; i<this.BlauSpace.Dimension; i++) {
				this.setSteps(i, orig.getSteps(i));   
			}
			
			reset();
			updateHashedName();
		}
		
		// clone method
		public IBlauSpaceIterator clone() {
			return new BlauSpaceIterator(this);
		}
		
		// stringifier
		public override string ToString ()
		{
			string s = "BlauSpaceReverseIterator @ (";
			for (int i=0; i<this.BlauSpace.Dimension; i++) {
				s+=(""+_coords[i]);
				if (i!=this.BlauSpace.Dimension-1) s+=", ";
			}
			s += ") in ";
			s += base.ToString();
			return s;
		}
	}
}

