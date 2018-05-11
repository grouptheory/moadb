using System;
using NUnit.Framework;
using logger;
using core;

namespace testing
{
	[TestFixture()]
	public class core_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(core_tests), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		[Test()]
		public void RandomGeneratorTest()
		{
			Console.WriteLine("RandomGeneratorTest");
			
			double mean = 10.0;
			double std = 0.6;
			
			for (int i=0; i<100; i++) {
				double v = SingletonRandomGenerator.Instance.NextGaussianPositive(mean, std);
				SingletonLogger.Instance().DebugLog(typeof(core_tests), "val = "+v);
				Console.WriteLine("val = "+v);
			}
			
			//throw new Exception();
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

