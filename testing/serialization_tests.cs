using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using System.Web;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Permissions;
using NUnit.Framework;
using logger;
using core;
using serialization;

namespace testing
{
	[TestFixture()]
	public class serialization_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(serialization_tests), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		[Test()]
		public void Test1()
		{
			SingletonLogger.Instance().DebugLog(typeof(serialization_tests), "Test1");
		
			FileStream fs1 = new FileStream("test.xml", FileMode.Create);
			SoapFormatter formatter = new SoapFormatter();
			
			Singleton s1 = Singleton.GetSingleton();
            try 
            {
    			//xmlser.Serialize(writer, s1);
				//writer.Flush();
				
				formatter.Serialize(fs1, s1);
    			
    			fs1.Flush();
    			fs1.Close();
            }
            catch (SerializationException e) 
            {
                Console.WriteLine("Serialization failed: {0}", e.Message);
                throw;
            }

            try 
            {
				FileStream fs2 = new FileStream("test.xml", FileMode.Open);
				//Singleton s2 = (Singleton) xmlser.Deserialize(fs2);
				Singleton s2 = (Singleton) formatter.Deserialize(fs2);
				fs2.Close();

                // Verify that it all worked.
                SingletonLogger.Instance().DebugLog(typeof(serialization_tests), "s1: "+s1);
                SingletonLogger.Instance().DebugLog(typeof(serialization_tests), "s2: "+s2);
                SingletonLogger.Instance().DebugLog(typeof(serialization_tests), "s1==s2: "+(s1==s2));
            }
            catch (SerializationException e) 
            {
                Console.WriteLine("Deserialization failed: {0}", e.Message);
                throw;
            }
        }
		
		[Test()]
		public void Test2()
		{
			SingletonLogger.Instance().DebugLog(typeof(serialization_tests), "Test2");
		}
		
		[Test()]
		public void Test3()
		{
			SingletonLogger.Instance().DebugLog(typeof(serialization_tests), "Test3");
		}
		
		[Test()]
		public void Test4()
		{
			SingletonLogger.Instance().DebugLog(typeof(serialization_tests), "Test4");
		}
	}
	
		[Serializable]
		public class Singleton : IObjectReference 
		{
		    // This is the one instance of this type.
		    private static readonly Singleton theOneObject = new Singleton(123);
			private int _id1;
		
		    // Private constructor allowing this type to construct the Singleton.
		    private Singleton(int id) 
		    { 
				_id1 = id;
		    }
		
			public Singleton() {
			}
		
			public int getID() {
				return _id1;
			}
		    // GetRealObject is called after this object is deserialized.
    		public Object GetRealObject(StreamingContext context) 
    		{
        		// When deserialiing this object, return a reference to 
        		// the Singleton object instead.
        		return Singleton.GetSingleton();
    		}
		
		    // A method returning a reference to the Singleton.
		    public static Singleton GetSingleton() 
		    { 
		        return theOneObject; 
		    }
		}
	
}

