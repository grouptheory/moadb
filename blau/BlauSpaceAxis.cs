using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;
using logger;

namespace blau
{
	/// <summary>
    /// BlauSpaceAxis class
    /// </summary>
	[Serializable]
	public class BlauSpaceAxis : IBlauSpaceAxis
	{
		// axis name
		private string _name;
		public string Name {
			get {return _name;}
		}
		
		// axis min value
		private double _minimumValue;
		public double MinimumValue {
			get {return _minimumValue;}
		}
		
		// axis max value
		private double _maximumValue;
		public double MaximumValue {
			get {return _maximumValue;}
		}
		
		// constructor
		public BlauSpaceAxis (string name, double min, double max)
		{
			this._name = name;
			this._minimumValue = min;
			this._maximumValue = max;
			if (_minimumValue > _maximumValue) {
				throw new Exception("BlauSpaceAxis _minimumValue > _maximumValue");
			}
		}
		
		// no arg ctor for deserialization
		public BlauSpaceAxis() {}
		
		// diagnostic to string
		public override string ToString ()
		{
			string s = "";
			s+=Name+"("+MinimumValue+","+MaximumValue+")";
			return s;
		}
	}
}

