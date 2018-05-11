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
	public class TrajectoryFactoryConfig : XmlSerializationIO<TrajectoryFactoryConfig>, ITrajectoryFactoryConfig
	{
		private string Value_Name;
		public string Name {
			get { return Value_Name; }
			set { Value_Name = value; }
		}
		
		private double Value_MinGranularity;
		public double MinGranularity {
			get { return Value_MinGranularity; }
			set { Value_MinGranularity = value; }
		}
		
		private double Value_HistoryCoefficient;
		public double HistoryCoefficient {
			get { return Value_HistoryCoefficient; }
			set { Value_HistoryCoefficient = value; }
		}
		
		public TrajectoryFactoryConfig () {
			Name = "UnknownTrajectoryFactory";
			MinGranularity = 1.0;
			HistoryCoefficient = 0.0;
		}
	}
}

