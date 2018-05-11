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
	public class TrajectoryFactorySetConfig : XmlSerializationIO<TrajectoryFactorySetConfig>, ITrajectoryFactorySetConfig
	{
		public static readonly string MODULE = "TrajectoryFactorySet";
		
		private List<TrajectoryFactoryConfig> _trajectoryConfigs;
		public List<ITrajectoryFactoryConfig> getTrajectories () {
			List<ITrajectoryFactoryConfig> converted = new List<ITrajectoryFactoryConfig>();
			foreach (TrajectoryFactoryConfig x in _trajectoryConfigs) converted.Add(x);
			return converted;
		}
		 
		public List<TrajectoryFactoryConfig> Trajectories {
			get { return _trajectoryConfigs; }
			set { _trajectoryConfigs = value; }
		}
		
		public TrajectoryFactorySetConfig () {
			_trajectoryConfigs = new List<TrajectoryFactoryConfig>();
		}
		
		// memoize
		[XmlIgnore()]
		private static Dictionary<string, TrajectoryFactorySetConfig> _cache = 
			new Dictionary<string, TrajectoryFactorySetConfig>();
			
		public static string FileName(String dir) {
			return Path.Combine(dir, "Config."+MODULE+".xml");
		}
		
		public static TrajectoryFactorySetConfig Factory(String dir) {
			string FILENAME = FileName(dir);
			
			if (_cache.ContainsKey(dir)) {
				return _cache[dir];
			}
			
			TrajectoryFactorySetConfig instance;
			try {
				instance = (TrajectoryFactorySetConfig) TrajectoryFactorySetConfig.Load(FILENAME);
				Console.WriteLine(":> TrajectoryFactorySetConfig "+FILENAME+" loaded.");
			}
			catch (System.IO.FileNotFoundException) {
				instance= new TrajectoryFactorySetConfig(); 
				instance.SaveAs(FILENAME);
				Console.WriteLine(":> TrajectoryFactorySetConfig "+FILENAME+" created");
			}
			
			_cache.Add(dir, instance);
			
			return instance;
		}
	}
}

