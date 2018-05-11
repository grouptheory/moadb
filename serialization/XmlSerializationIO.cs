using System;
using System.IO;
using System.Xml.Serialization;

namespace serialization
{
	public class XmlSerializationIO <classType>
	{
		public XmlSerializationIO()
		{
		}
		
	    static public classType Load(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(classType));
            classType classObj = (classType)serializer.Deserialize(fileStream);
			
            fileStream.Close();
            return classObj;
        }

        public void SaveAs(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(classType));
            serializer.Serialize(fileStream, this);
            fileStream.Close();
        }
	}
}
