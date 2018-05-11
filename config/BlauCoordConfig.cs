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
	public class BlauCoordConfig: XmlSerializationIO<BlauCoordConfig>, IBlauCoordConfig
	{
		private string Value_AxisName;
		public string AxisName {
			get { return Value_AxisName; }
			set { Value_AxisName = value; }
		}
		
		private double Value_Value;
		public double Value {
			get { return Value_Value; }
			set { Value_Value = value; }
		}
		
		public BlauCoordConfig () {
		}
		
		public BlauCoordConfig (IBlauAxisConfig axis) {
			AxisName = axis.Name;
			Value = axis.MinimumValue;
		}
	}
}

