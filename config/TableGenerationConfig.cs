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
	public class TableGenerationConfig : XmlSerializationIO<TableGenerationConfig>, ITableGenerationConfig
	{
		public static readonly string MODULE = "TableGeneration";
		
		public static string DEFAULT_AgentFactoryClassName = "Agent0x0_Factory";
		public static int DEFAULT_NumAgents = 20;
		public static double DEFAULT_DurationHours = 1.0;
		public static int DEFAULT_Trials = 10;
		public static int DEFAULT_Populations = 5;
		static public readonly string DEFAULT_InitialOrderbook = "orderbooks/orderbook.csv";
		
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
		
		private int Value_Populations;
		public int Populations {
			get { return Value_Populations; }
			set { Value_Populations = value; }
		}
		
		private int Value_Trials;
		public int Trials {
			get { return Value_Trials; }
			set { Value_Trials = value; }
		}
		
		private int Value_NumCombs;
		public int NumCombs {
			get { return Value_NumCombs; }
			set { Value_NumCombs = value; }
		}
		
		private double Value_Lambda;
		public double Lambda {
			get { return Value_Lambda; }
			set { Value_Lambda = value; }
		}
		
		private double Value_Gamma;
		public double Gamma {
			get { return Value_Gamma; }
			set { Value_Gamma = value; }
		}
		
		private string Value_InitialOrderbook;
		public string InitialOrderbook {
			get { return ""+ApplicationConfig.ROOTDIR+Value_InitialOrderbook; }
			set { Value_InitialOrderbook = value; }
		}
		
		private double Value_BurnInTimeHours;
		public double InitialBurninHours {
			get { return Value_BurnInTimeHours; }
			set { Value_BurnInTimeHours = value; }
		}
		
		public TableGenerationConfig () {
			AgentFactoryClassName = DEFAULT_AgentFactoryClassName;
			NumAgents = DEFAULT_NumAgents;
			DurationHours = DEFAULT_DurationHours;
			Trials = DEFAULT_Trials;
			Populations = DEFAULT_Populations;
			InitialOrderbook = DEFAULT_InitialOrderbook;
		}
		
		// memoize
		[XmlIgnore()]
		private static Dictionary<string, TableGenerationConfig> _cache = 
			new Dictionary<string, TableGenerationConfig>();
		
		public static string FileName(String dir) {
			return Path.Combine(dir, "Config."+MODULE+".xml");
		}
		
		public static TableGenerationConfig Factory(String dir) {
			string FILENAME = FileName(dir);
			
			if (_cache.ContainsKey(dir)) {
				return _cache[dir];
			}
			
			TableGenerationConfig instance;
			try {
				instance = (TableGenerationConfig) TableGenerationConfig.Load(FILENAME);
				Console.WriteLine(":> TableGenerationConfig "+FILENAME+" loaded.");
			}
			catch (System.IO.FileNotFoundException) {
				instance= new TableGenerationConfig(); 
				instance.SaveAs(FILENAME);
				Console.WriteLine(":> TableGenerationConfig "+FILENAME+" created");
			}
			
			_cache.Add(dir, instance);
			
			return instance;
		}
	}
}

