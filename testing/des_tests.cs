using System;
using NUnit.Framework;
using logger;
using core;
using des;

namespace testing
{
	[TestFixture()]
	public class des_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(des_tests), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		[Test()]
		public void Test1()
		{
			Console.WriteLine("Test1");
		}
		
		[Test()]
		public void Test2()
		{
			Console.WriteLine("Test2");
		}
		
		[Test()]
		public void Test3()
		{
			Console.WriteLine("Test3");
		}
		
		[Test()]
		public void Test4()
		{
			Console.WriteLine("Test4");
		}
	}
}

