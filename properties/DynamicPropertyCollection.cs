using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using serialization;

namespace properties
{
	[Serializable]
    public class DynamicPropertyCollection : XmlSerializationIO<DynamicPropertyCollection>
	{
		
        private List<DynamicProperty> _dynamicProperties;
        private Dictionary<string, DynamicProperty> _dynamicPropertiesDictionary;

		public DynamicPropertyCollection()
		{
		}
		
        public List<DynamicProperty> Properties
        {
            get { 
				if (this._dynamicProperties == null) {
					this._dynamicProperties = new List<DynamicProperty>();
				}
				return this._dynamicProperties; 
			}
            set 
            { 
                this._dynamicProperties = value;
                CopyToDictionary();
            }
        }

        private void CopyToDictionary()
        {
            this._dynamicPropertiesDictionary = new Dictionary<string, DynamicProperty>();
            foreach (DynamicProperty dynamicProperty in this._dynamicProperties)
            {
                this._dynamicPropertiesDictionary.Add(dynamicProperty.Name, dynamicProperty);
            }
        }
		
		public string ReadProperty(string name)
		{
			return _dynamicPropertiesDictionary[name].Value;
		}
		
		public void WriteToProperty(string name, string newValue)
		{
			_dynamicPropertiesDictionary[name].Value=newValue;
		}

		public bool ContainsProperty(string name)
		{
			return _dynamicPropertiesDictionary.ContainsKey(name);
		}
		
        [XmlIgnore]
        private Dictionary<string,DynamicProperty> PropertiesDictionary
        {
            get 
            {
                if ((this._dynamicPropertiesDictionary == null) || (this._dynamicPropertiesDictionary.Count != this._dynamicProperties.Count))
                {
                    CopyToDictionary();
                }
                return this._dynamicPropertiesDictionary; 
            }
        }
    }
}
