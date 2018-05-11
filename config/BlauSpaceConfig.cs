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
	public class BlauSpaceConfig : XmlSerializationIO<BlauSpaceConfig>, IBlauSpaceConfig
	{
		private int Value_Dimensions;
		public int Dimensions {
			get { return Value_Dimensions; }
			set { Value_Dimensions = value; }
		}
		
		List<BlauAxisConfig> _axisConfigs;
		public List<IBlauAxisConfig> getAxes() {
			List<IBlauAxisConfig> converted = new List<IBlauAxisConfig>();
			foreach (IBlauAxisConfig x in _axisConfigs) converted.Add(x);
			return converted;
		}
		public List<BlauAxisConfig> Axes {
			get { return _axisConfigs; }
			set { _axisConfigs = value; }
		}
		
		public BlauSpaceConfig () {
			Dimensions = 0;
			_axisConfigs = new List<BlauAxisConfig>();
		}
		
		public BlauSpaceConfig (int dim) {
			Dimensions = dim;
			_axisConfigs = new List<BlauAxisConfig>();
			for (int i=0; i<Dimensions; i++) {
				_axisConfigs.Add(new BlauAxisConfig(i));
			}
		}
	}
}

