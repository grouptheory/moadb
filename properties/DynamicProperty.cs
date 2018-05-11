using System;
using System.Xml.Serialization;

namespace properties
{
	[Serializable]
	public class DynamicProperty
	{
		
        private string _name;
        private string _value;

        public DynamicProperty() { }

        public DynamicProperty(string name, string value)
        {
            this._name = name;
            this._value = value;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        [XmlAttribute("value")]
        public string Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

	}
}
