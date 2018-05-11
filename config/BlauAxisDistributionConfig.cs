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
	public class BlauAxisDistributionConfig : XmlSerializationIO<BlauAxisConfig>, IBlauAxisDistributionConfig
	{
		private string Value_Name;
		public string Name {
			get { return Value_Name; }
			set { Value_Name = value; }
		}
		
		private string Value_File;
		public string File {
			get { return Value_File; }
			set { Value_File = value; }
		}
		
		public BlauAxisDistributionConfig () {
		}
		
		public BlauAxisDistributionConfig (int i) {
			Name = "X"+i;
			File = "File-"+i;
		}
		
		public override string ToString ()
		{
			string s = "";
			s+=("Name:"+Name);
			s+=(" ");
			s+=("File:"+File);
			return s;
		}
	}
}

