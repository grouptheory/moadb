using System;
using NUnit.Framework;
using logger;
using core;
using agent;
using models;
using blau;
using dist;

namespace testing
{
	[TestFixture()]
	public class agent_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(agent_tests), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		[Test()]
		public void PopulationFactoryTest()
		{
			Console.WriteLine("PopulationFactoryTest");
			
			int dimP = 3;
			string [] namesP = new string [3] {"x0", "x1", "x2"};
			double [] minsP = new double [3] {0.0, 0.0, 0.0};
			double [] maxsP = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace sP = BlauSpace.create(dimP, namesP, minsP, maxsP);
			Product d = new Product(sP);
			
			for (int i=0;i<3;i++) {
				int dim = 1;
				string [] names = new string [1];
				names[0] = "x"+i;
				double [] mins = new double [1] {0.00};
				double [] maxs = new double [1] {100.0};
				IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
				double mean = 10.0 * (i+1.0);
				double std = i+1.0;
				IDistribution di = new Distribution_Gaussian(s, mean, std);
				d.Add (di);
			}
			
			d.DistributionComplete();

			IPopulationFactory pf = PopulationFactory.Instance();
			int POPSIZE = 100;

			AgentDummy_Factory adf = new AgentDummy_Factory(d);

			IPopulation pop = pf.create(adf, POPSIZE);
			Assert.AreEqual(pop.Size, POPSIZE);
			
			SingletonLogger.Instance().DebugLog(typeof(agent_tests), "distribution: "+d);
			SingletonLogger.Instance().DebugLog(typeof(agent_tests), "pop: \n"+pop);
			
			int count=0;
			foreach (IAgent ag in pop) {
				for (int x=0;x<3;x++) {
					double diff = Math.Abs(ag.Coordinates.getCoordinate(x)- (10.0 * (x+1.0)));
					Assert.AreEqual((diff > 5.0* (x+1.0)), false);
				}
				count++;
			}
			Assert.AreEqual(count, POPSIZE);
			
			for (int j=0;j<count;j++) {
				IAgent agj = pop.getAgent(j);
				Assert.Throws<Exception>( delegate { pop.addAgent(agj); } );
			}
			
			for (int j=0;j<count;j++) {
				IAgent agj = pop.getAgent(0);
				pop.removeAgent(agj);
				Assert.AreEqual(pop.Size, POPSIZE-1);
				Assert.Throws<Exception>( delegate { pop.removeAgent(agj); } );
				pop.addAgent(agj);
			}
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

