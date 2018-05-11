using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using serialization;
using properties;
using core;

namespace config
{
	[Serializable]
	public class BlauAxisConfig : XmlSerializationIO<BlauAxisConfig>, IBlauAxisConfig
	{
		private int Value_Index;
		public int Index {
			get { return Value_Index; }
			set { Value_Index = value; }
		}
		
		private string Value_Name;
		public string Name {
			get { return Value_Name; }
			set { Value_Name = value; }
		}
		
		private double Value_Min;
		public double MinimumValue {
			get { return Value_Min; }
			set { Value_Min = value; }
		}
		
		private double Value_Max;
		public double MaximumValue {
			get { return Value_Max; }
			set { Value_Max = value; }
		}
		
		public BlauAxisConfig () {
		}
		
		public BlauAxisConfig (int i) {
			Index = i;
			Name = "X"+i;
			MinimumValue = 0.0;
			MaximumValue = 1.0;
		}
	}
}

