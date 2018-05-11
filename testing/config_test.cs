using System;
using NUnit.Framework;
using config;
using logger;

namespace testing
{
	[TestFixture()]
	public class config_test
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(config_test), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		[Test()]
		public void TestLoad()
		{
			TrajectoryFactorySetConfig conf2 = TrajectoryFactorySetConfig.Factory(ApplicationConfig.EXECDIR);
			SingletonLogger.Instance().DebugLog(typeof(config_test), "TrajectoryFactorySetConfig => "+conf2);

			AgentEvaluationFactorySetConfig conf3 = AgentEvaluationFactorySetConfig.Factory(ApplicationConfig.EXECDIR);
			SingletonLogger.Instance().DebugLog(typeof(config_test), "AgentEvaluationFactorySetConfig => "+conf3);
		}
	}
}

