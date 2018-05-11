using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace blau
{	
	/// <summary>
	/// QuantizedBlauPoint class
	/// </summary>
	[Serializable]
	public class QuantizedBlauPoint : BlauPoint
	{	
		private IBlauSpaceLattice _quantizer;
		
		private int [] _quantizedCoords;
		
		private int getQuantizedCoordinate(int i) {
			return _quantizedCoords[i];
		}
		
		private void setQuantizedCoordinate(int i, int val) {
			_quantizedCoords[i] = val;
		}
		
		public override void setCoordinate(int i, double val) {
			int newval = (int) Math.Round((val-_quantizer.BlauSpace.getAxis(i).MinimumValue)/_quantizer.getStepSize(i));
			
			//Console.WriteLine("XXX "+val+" "+(i)+" ==Q==> ["+_quantizer.BlauSpace.getAxis(i).MinimumValue+":"+_quantizer.getStepSize(i));
			
			setQuantizedCoordinate(i, newval);
			base.setCoordinate(i, _quantizedCoords[i]*_quantizer.getStepSize(i) + _quantizer.BlauSpace.getAxis(i).MinimumValue);
		}
		
		public override IBlauPoint clone() {
			IBlauPoint dupe = new QuantizedBlauPoint(this.Space, this._quantizer);
			for (int i=0; i<this.Space.Dimension; i++) {
				dupe.setCoordinate(i, this.getCoordinate(i));
			}
			return dupe;
		}
		
		public override int CompareTo(object obj) {
	        if (obj is QuantizedBlauPoint) {
	            QuantizedBlauPoint p = (QuantizedBlauPoint)obj;
				for (int i=0; i<this.Space.Dimension; i++) {
					if (this.getQuantizedCoordinate(i) < p.getQuantizedCoordinate(i)) return -1;
					if (this.getQuantizedCoordinate(i) > p.getQuantizedCoordinate(i)) return +1;
				}
				return 0;
	        }
			else if (obj is BlauPoint) {
				return base.CompareTo(obj);
			}
	        throw new ArgumentException("object is not a QuantizedBlauPoint/BlauPoint");    
	    }
		
		/*
		protected override int RecalculateHashCode() {
	        int val = 0;
			int digit = 1;
			for (int i=0; i<this.Space.Dimension; i++) {
				val += getQuantizedCoordinate(i) * digit;
				val = val % Int16.MaxValue;
				
				digit *= _quantizer.getSteps(i);
				digit = digit % Int16.MaxValue;
			}
			return val;
		}
		*/
		
		public QuantizedBlauPoint(IBlauSpace space, IBlauSpaceLattice quantizer) : base(space)
		{
			_quantizer = quantizer;
			_quantizedCoords = new int [space.Dimension];
			for (int i=0; i<space.Dimension; i++) {
				this.setQuantizedCoordinate(i, 0);
			}
		}
		
		public override string ToString ()
		{
			string s = base.ToString();
			s += " <quantized>";
			return s;
		}
		
	}
}

