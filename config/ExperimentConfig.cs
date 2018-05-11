using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using serialization;
using properties;
using core;

namespace config
{
	[Serializable]
	public class ExperimentConfig : XmlSerializationIO<ExperimentConfig>, IExperimentConfig
	{
		public static readonly string MODULE = "Experiment";
		
		public static int DEFAULT_NumAgents = 20;
		public static double DEFAULT_DurationHours = 1.0;
		public static int DEFAULT_Trials = 10;
		static public readonly string DEFAULT_InitialOrderbook = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
		
		private string Value_FactoryName;
		public string AgentFactoryClassName {
			get { return Value_FactoryName; }
			set { Value_FactoryName = value; }
		}
		
		private int Value_NumAgents;
		public int NumAgents {
			get { return Value_NumAgents; }
			set { Value_NumAgents = value; }
		}
		
		private double Value_DurationHours;
		public double DurationHours {
			get { return Value_DurationHours; }
			set { Value_DurationHours = value; }
		}
		
		private int Value_Trials;
		public int Trials {
			get { return Value_Trials; }
			set { Value_Trials = value; }
		}
		
		private string Value_InitialOrderbook;
		public string InitialOrderbook {
			get { return Value_InitialOrderbook; }
			set { Value_InitialOrderbook = value; }
		}
		
		public ExperimentConfig () {
			NumAgents = DEFAULT_NumAgents;
			DurationHours = DEFAULT_DurationHours;
			Trials = DEFAULT_Trials;
			InitialOrderbook = DEFAULT_InitialOrderbook;
		}
		
		// memoize
		[XmlIgnore()]
		private static Dictionary<string, ExperimentConfig> _cache = 
			new Dictionary<string, ExperimentConfig>();
			
		public static ExperimentConfig Factory(String dir) {
			string FILENAME = dir+"Config."+MODULE+".xml";
			
			if (_cache.ContainsKey(dir)) {
				return _cache[dir];
			}
			
			ExperimentConfig instance;
			try {
				instance = (ExperimentConfig) ExperimentConfig.Load(FILENAME);
				Console.WriteLine(":> ExperimentConfig "+FILENAME+" loaded.");
			}
			catch (System.IO.FileNotFoundException) {
				instance= new ExperimentConfig(); 
				instance.SaveAs(FILENAME);
				Console.WriteLine(":> ExperimentConfig "+FILENAME+" created");
			}
			
			_cache.Add(dir, instance);
			
			return instance;
		}
	}
}

