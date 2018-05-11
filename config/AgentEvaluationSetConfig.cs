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
	public class AgentEvaluationFactorySetConfig : XmlSerializationIO<AgentEvaluationFactorySetConfig>, IAgentEvaluationFactorySetConfig
	{
		public static readonly string MODULE = "AgentEvaluationFactorySet";
		
		List<AgentEvaluationConfig> _evaluationConfigs;
		public List<IAgentEvaluationConfig>  getAgentEvaluations() {
			List<IAgentEvaluationConfig> converted = new List<IAgentEvaluationConfig>();
			foreach (IAgentEvaluationConfig x in _evaluationConfigs) converted.Add(x);
			return converted;
		}
		
		public List<AgentEvaluationConfig> AgentEvaluations {
			get { return _evaluationConfigs; }
			set { _evaluationConfigs = value; }
		}
		
		public AgentEvaluationFactorySetConfig () {
			_evaluationConfigs = new List<AgentEvaluationConfig>();
		}
		
		// memoize
		[XmlIgnore()]
		private static Dictionary<string, AgentEvaluationFactorySetConfig> _cache = 
			new Dictionary<string, AgentEvaluationFactorySetConfig>();
			
		public static string FileName(String dir) {
			return Path.Combine(dir, "Config."+MODULE+".xml");
		}
		
		public static AgentEvaluationFactorySetConfig Factory(String dir) {
			string FILENAME = FileName(dir);
			
			if (_cache.ContainsKey(dir)) {
				return _cache[dir];
			}
			
			AgentEvaluationFactorySetConfig instance;
			try {
				instance = (AgentEvaluationFactorySetConfig) AgentEvaluationFactorySetConfig.Load(FILENAME);
				Console.WriteLine(":> AgentEvaluationFactorySetConfig "+FILENAME+" loaded.");
			}
			catch (System.IO.FileNotFoundException) {
				instance= new AgentEvaluationFactorySetConfig(); 
				instance.SaveAs(FILENAME);
				Console.WriteLine(":> AgentEvaluationFactorySetConfig "+FILENAME+" created");
			}
			
			_cache.Add(dir, instance);
			
			return instance;
		}
	}
}

