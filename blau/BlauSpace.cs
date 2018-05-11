using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;
using logger;
using serialization;

namespace blau
{
	/// <summary>
    /// BlauSpace class
    /// </summary>
	[Serializable]
	public class BlauSpace : IBlauSpace, IComparable, IObjectReference 
	{
		// used as key in blauspace registry to avoid multiple instances of identical blauspaces upon deserialization
		private string _hashedName;
		public string HashedName {
			get {return _hashedName;}
		}
		
		// blauspace dimension
		private int _dimension;
		public int Dimension {
			get {return _dimension;}
		}
		
		// get axis by index
		public IBlauSpaceAxis getAxis(int i) {
			if ((i<0) || (i>=_dimension)) {
				throw new Exception("Attempt to get illegal axis out of BlauSpace dimensionality range");
			}
			else {
				return (IBlauSpaceAxis)_indexedAxes[i];
			}
		}
		
		// get axis by name
		public int getAxisIndex(string name) {
			for (int i=0; i<this.Dimension; i++) {
				BlauSpaceAxis axis = (BlauSpaceAxis)_indexedAxes[i];
				if (axis.Name.Equals(name)) {
					return i;
				}
			}
			throw new Exception("BlauSpace does not contain axis: "+name);
		}
		
		// test axis presence, by name
		public bool hasAxis(string name) {
			for (int i=0; i<this.Dimension; i++) {
				BlauSpaceAxis axis = (BlauSpaceAxis)_indexedAxes[i];
				if (axis.Name.Equals(name)) {
					return true;
				}
			}
			return false;
		}
		
		// index to axis
		private Hashtable _indexedAxes;
		
		// blauspace comparator generic
		public int CompareTo(object obj) {
        	if (obj == null) return 1;
			
        	BlauSpace bs = obj as BlauSpace;
        	if (bs != null) 
            	return this.CompareTo(bs);
        	else
           		throw new ArgumentException("Object is not a BlauSpace");
    	}
		
		// blauspace comparator specific
		public int CompareTo(BlauSpace bs) {
			int c1 = (this.Dimension < bs.Dimension) ? -1 : ((this.Dimension == bs.Dimension) ? 0 : +1);
			if (c1 != 0) return c1;
			
			BlauSpaceAxisComparer cmp = new BlauSpaceAxisComparer();
			
			if (_indexedAxes == null) {
           		throw new ArgumentException("_indexedAxes is null");
			}
			if (bs._indexedAxes == null) {
           		throw new ArgumentException("bs._indexedAxes is null");
			}
			
			SortedDictionary<BlauSpaceAxis, BlauSpaceAxis> thisAxes = new SortedDictionary<BlauSpaceAxis, BlauSpaceAxis>(cmp);
			foreach (int x in _indexedAxes.Keys) {
				thisAxes.Add ((BlauSpaceAxis)_indexedAxes[x], (BlauSpaceAxis)_indexedAxes[x]);
			}
			SortedDictionary<BlauSpaceAxis, BlauSpaceAxis> bsAxes = new SortedDictionary<BlauSpaceAxis, BlauSpaceAxis>(cmp);
			foreach (int x in bs._indexedAxes.Keys) {
				bsAxes.Add ((BlauSpaceAxis)bs._indexedAxes[x], (BlauSpaceAxis)bs._indexedAxes[x]);
			}
			
			IEnumerator<BlauSpaceAxis> thisAxesEnum = thisAxes.Keys.GetEnumerator();
			IEnumerator<BlauSpaceAxis> bsAxesEnum = bsAxes.Keys.GetEnumerator();
			while (thisAxesEnum.MoveNext()) {
				BlauSpaceAxis x1 = thisAxesEnum.Current;
				
				bsAxesEnum.MoveNext();
				BlauSpaceAxis x2 = bsAxesEnum.Current;
				int c2 = cmp.Compare(x1,x2);
				if (c2 != 0) return c2;
			}
			return 0;
    	}
		
		// check if one BlauSpace is contained in another
		public static bool contains(IBlauSpace big, IBlauSpace small) {
			for (int i=0; i<small.Dimension;i++) {
					//Console.Out.WriteLine("  checking if big space has : "+small.getAxis(i).Name);
				
				if (big.hasAxis(small.getAxis(i).Name)) {
					int j = big.getAxisIndex(small.getAxis(i).Name);
					
					if ((big.getAxis(j).MinimumValue <= small.getAxis(i).MinimumValue) &&
					    (big.getAxis(j).MaximumValue >= small.getAxis(i).MaximumValue)) {
						//Console.Out.WriteLine("  in bounds");
						continue;
					}
					else {
						//Console.Out.WriteLine("  out of bounds");
						//Console.Out.WriteLine("  min big "+big.getAxis(j).MinimumValue);
						//Console.Out.WriteLine("  min small "+small.getAxis(i).MinimumValue);
						//Console.Out.WriteLine("  max big "+big.getAxis(j).MaximumValue);
						//Console.Out.WriteLine("  max small "+small.getAxis(i).MaximumValue);
						return false;
					}
				}
				else {
					return false;
				}
			}
			return true;
		}
		
		// check if two BlauSpaces have a nontrivial intersection
		public static bool intersects(IBlauSpace s1, IBlauSpace s2) {
			for (int i=0; i<s1.Dimension;i++) {
				if (s2.hasAxis(s1.getAxis(i).Name)) return true;
				else continue;
			}
			return false;
		}
		
		// deserializer consults the blauspace registry to avoid duplicate instantiations
		public Object GetRealObject(StreamingContext context)  {
			IBlauSpace s_validated = BlauSpaceRegistry.Instance().validate(this);
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(BlauSpace), "VALIDATING BLAUSPACE");
			return s_validated;
    	}
		
		// get minimal point
		public IBlauPoint getMinimalPoint() {
			IBlauPoint bp = new BlauPoint(this);
			for (int i=0; i<this.Dimension;i++) {
				bp.setCoordinate(i, this.getAxis(i).MinimumValue);
			}
			return bp;
		}
		
		// static factory method, which consults registry to avoid multiple instantiations
		public static IBlauSpace create(int dimension, string [] names, double [] min, double [] max) {
			BlauSpace s = new BlauSpace(dimension, names, min, max);
			// register new instances with the registry
			BlauSpaceRegistry.Instance().add(s);
			// query the registry for canonical representative
			IBlauSpace s_validated = BlauSpaceRegistry.Instance().validate(s);
			return s_validated;
		}
		
		// private constructor
		private BlauSpace (int dimension, string [] names, double [] min, double [] max)
		{
			/*
			if ((dimension != names.Length) || 
			    (dimension != min.Length) || 
			    (dimension != max.Length)) {
				throw new Exception("BlauSpace ctor received an inconsistent dimensional description ");
			}
			*/

			_dimension = dimension;
			_indexedAxes = new Hashtable();
			
			for (int i=0; i<dimension; i++) {
				BlauSpaceAxis axis_i = new BlauSpaceAxis(names[i], min[i], max[i]);
				_indexedAxes.Add(i, axis_i);
			}
			
			_hashedName = this.ToString ();
		}
		
		// diagnostic to string
		public override string ToString ()
		{
			string s = "BlauSpace[Dim:"+Dimension+"]: ";
			for (int i=0; i<Dimension; i++) {
				s+=getAxis(i).ToString();
				if (i!=Dimension-1) s+=", ";
			}
			//s+="hash:"+this.GetHashCode();
			
			return s;
		}
	
		// register deserialized instances with the registry
	    [OnDeserialized()]
	    internal void register(StreamingContext context)
	    {
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(BlauSpace), "REGISTERING BLAUSPACE");
			BlauSpaceRegistry.Instance().add(this);
	    }
	}
}

