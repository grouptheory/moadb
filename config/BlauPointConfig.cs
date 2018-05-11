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
	public class BlauPointConfig : XmlSerializationIO<BlauPointConfig>, IBlauPointConfig
	{
		private int Value_Dimensions;
		public int Dimensions {
			get { return Value_Dimensions; }
			set { Value_Dimensions = value; }
		}
		
		List<BlauCoordConfig> _Coords;
		public List<IBlauCoordConfig> getCoords() {
			List<IBlauCoordConfig> converted = new List<IBlauCoordConfig>();
			foreach (IBlauCoordConfig x in _Coords) converted.Add(x);
			return converted;
		}
		public List<BlauCoordConfig> Coords {
			get { return _Coords; }
			set { _Coords = value; }
		}
		
		public BlauPointConfig () {
		}
		
		public BlauPointConfig(IBlauSpaceConfig space) {
			Dimensions = space.Dimensions;
			_Coords = new List<BlauCoordConfig>();
			foreach (IBlauAxisConfig xc in space.getAxes()) {
				_Coords.Add(new BlauCoordConfig(xc));
			}
		}
	}
}

