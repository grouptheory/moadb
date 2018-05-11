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
	public class AgentEvaluationConfig : XmlSerializationIO<AgentEvaluationConfig>, IAgentEvaluationConfig
	{
		private string Value_Name;
		public string Name {
			get { return Value_Name; }
			set { Value_Name = value; }
		}
		
		private string Value_MetricName;
		public string MetricName {
			get { return Value_MetricName; }
			set { Value_MetricName = value; }
		}
		
		private int Value_BlauSpaceGridding;
		public int BlauSpaceGridding {
			get { return Value_BlauSpaceGridding; }
			set { Value_BlauSpaceGridding = value; }
		}
		
		public AgentEvaluationConfig () {
			Name = "UnknownAgentEvaluationFactory";
			MetricName = "UnknownMetricName";
			BlauSpaceGridding = 1;
		}
	}
}

