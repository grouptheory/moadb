using System;
using NUnit.Framework;
using logger;
using core;
using blau;
using metrics;
using models;
using dist;
using sim;
using agent;
using orderbook;
using des;
using signal;
using config;

namespace testing
{
	[TestFixture()]
	public class sim_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		private AgentOrderbookLoader MakeAgentOrderbookLoader(string PATH) {
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			IBlauPoint mean = new BlauPoint(s);
			IBlauPoint std = new BlauPoint(s);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new AgentOrderbookLoader_Factory(PATH, d);
			IPopulation pop = PopulationFactory.Instance().create(afact, 1);
			
			AgentOrderbookLoader loader = null;
			foreach (IAgent ag in pop) {
				loader = (AgentOrderbookLoader)ag;
				break;
			}
			
			return loader;
		}
		
		[Test()]
		public void DummySimulation1()
		{
			Console.WriteLine("DummySimulation1");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(AbstractAgent), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(AgentDummy), LogLevel.Info);
			
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			IBlauPoint mean = new BlauPoint(s);
			mean.setCoordinate(0, 10.0);
			mean.setCoordinate(1, 20.0);
			mean.setCoordinate(2, 30.0);
			
			IBlauPoint std = new BlauPoint(s);
			std.setCoordinate(0, 2.0);
			std.setCoordinate(1, 4.0);
			std.setCoordinate(2, 6.0);
			
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new AgentDummy_Factory(d);
			int NUMAGENTS = 100;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			IOrderbook_Observable ob = new Orderbook();
			
			string PROPERTY = "NetWorth";
			IAgentEvaluationBundle aeb = new AgentEvaluationBundle(PROPERTY);
							
			ISimulation sim = new Simulation(pop.clone(), ob.clone(), 0.0, 100.0);
			NamedMetricAgentEvaluationFactory metricEF = new NamedMetricAgentEvaluationFactory(PROPERTY);
			sim.add (metricEF);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResults res = sim.run();
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
				
			IAgentEvaluation ae = metricEF.create();
			
			aeb.addAgentEvaluation(ae);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "aeb: "+aeb.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "res: "+res.ToStringLong());
			
			Assert.AreEqual(res.Valid, true);
		}
		
		[Test()]
		public void AgentOrderbookLoaderTest()
		{
			Console.WriteLine("AgentOrderbookLoaderTest");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(AbstractAgent), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(AgentOrderbookLoader), LogLevel.Info);

			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "AgentOrderbookLoader: "+loader);
			
			IOrderbook_Observable ob = new Orderbook();
			IPopulation pop = new Population();
			pop.addAgent(loader);
			
			ISimulation sim = new Simulation(pop, ob, 0.0, 100.0);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResults res = sim.run();
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "ob: "+ob.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "res: "+res.ToStringLong());
			
			Assert.AreEqual(res.Valid, true);
		}
		
		[Test()]
		public void Agent0x1Simulation1()
		{
			Console.WriteLine("Agent0x1Simulation1");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(AbstractAgent), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(AgentOrderbookLoader), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(Agent0x0), LogLevel.Info);
			
			int dim = 1;
			string[] names = new string [1] {"x"};
			double[] mins = new double [1] {0.0};
			double[] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			IBlauPoint mean = new BlauPoint(s);
			mean.setCoordinate(0, 10.0);
			IBlauPoint std = new BlauPoint(s);
			std.setCoordinate(0, 2.0);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new Agent0x0_Factory(d);
			int NUMAGENTS = 10;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			pop.addAgent(loader);
			
			IOrderbook_Observable ob = new Orderbook();
			
			string PROPERTY = "NetWorth";
			IAgentEvaluationBundle aeb = new AgentEvaluationBundle(PROPERTY);
			
			// 1 hours
			ISimulation sim = new Simulation(pop, ob, 0.0, 3600.0);
			NamedMetricAgentEvaluationFactory metricEF = new NamedMetricAgentEvaluationFactory(PROPERTY);
			sim.add (metricEF);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResults res = sim.run();
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
				
			IAgentEvaluation ae = metricEF.create();
			
			aeb.addAgentEvaluation(ae);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "ob: "+ob.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "aeb: "+aeb.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "res: "+res.ToStringLong());
			
			Assert.AreEqual(res.Valid, true);
		}

		[Test()]
		public void Agent0x1Simulation_ZeroDimensional()
		{
			Console.WriteLine("Agent0x1Simulation_ZeroDimensional");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(AbstractAgent), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(AgentOrderbookLoader), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(Agent0x0), LogLevel.Info);
			
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauPoint mean = new BlauPoint(s);
			IBlauPoint std = new BlauPoint(s);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new Agent0x0_Factory(d);
			int NUMAGENTS = 10;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			pop.addAgent(loader);
			
			IOrderbook_Observable ob = new Orderbook();
			
			string PROPERTY = "NetWorth";
			IAgentEvaluationBundle aeb = new AgentEvaluationBundle(PROPERTY);
			
			// 1 hours
			ISimulation sim = new Simulation(pop, ob, 0.0, 3600.0);
			NamedMetricAgentEvaluationFactory metricEF = new NamedMetricAgentEvaluationFactory(PROPERTY);
			sim.add (metricEF);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResults res = sim.run();
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
				
			IAgentEvaluation ae = metricEF.create();
			
			aeb.addAgentEvaluation(ae);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "ob: "+ob.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "aeb: "+aeb.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "res: "+res.ToStringLong());
			
			Assert.AreEqual(res.Valid, true);
		}
		
		[Test()]
		public void Agent0x1Simulation_MultiTrial()
		{
			Console.WriteLine("Agent0x1Simulation_MultiTrial");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(AbstractAgent), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(AgentOrderbookLoader), LogLevel.Info);
			LoggerInitialization.SetThreshold(typeof(Agent0x0), LogLevel.Info);
			
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauPoint mean = new BlauPoint(s);
			IBlauPoint std = new BlauPoint(s);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new Agent0x0_Factory(d);
			int NUMAGENTS = 10;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			pop.addAgent(loader);
			
			IOrderbook_Observable ob = new Orderbook();
			
			string PROPERTY = "NetWorth";
			
			// 1 hours
			ISimulationBundle simb = new SimulationBundle(pop, ob, 0.0, 3600.0);
			NamedMetricAgentEvaluationFactory metricEF = new NamedMetricAgentEvaluationFactory(PROPERTY);
			simb.add (metricEF);
			
			int NUMTRIALS = 8;
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResultsBundle resb = simb.run(NUMTRIALS);
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
			
			int STEPS = 1;
			int [] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			
			foreach (IAgentEvaluationBundle aeb in resb.getAgentEvaluationBundles()) {
				IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
				IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "meanEval: "+meanEval.ToStringLong());
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "stdEval: "+stdEval.ToStringLong());
			}
			
			Assert.AreEqual(resb.Valid, true);
		}
		
		
		[Test()]
		public void Agent0x1Simulation_Trajectories()
		{
			Console.WriteLine("Agent0x1Simulation_Trajectories");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Agent0x0), LogLevel.Info);
			//LoggerInitialization.SetThreshold(typeof(SimulationBundle), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Trajectory), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(TrajectoryFactory_Price), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Orderbook), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Matcher), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Simulation), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(AbstractTrajectoryFactory), LogLevel.Debug);
			
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauPoint mean = new BlauPoint(s);
			IBlauPoint std = new BlauPoint(s);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new Agent0x0_Factory(d);
			int NUMAGENTS = 10;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			pop.addAgent(loader);
			
			IOrderbook_Observable ob = new Orderbook();
			
			string PROPERTY = "NetWorth";
			IAgentEvaluationBundle aeb = new AgentEvaluationBundle(PROPERTY);
			
			// 1 hours
			ISimulation sim = new Simulation(pop, ob, 0.0, 3600.0);
			
			IAgentEvaluationFactory metricEF = new NamedMetricAgentEvaluationFactory(PROPERTY);
			sim.add (metricEF);
			
			ITrajectoryFactory priceTF = new TrajectoryFactory_Price(10.0, 0.8);
			sim.add(priceTF);
			
			ITrajectoryFactory spreadTF = new TrajectoryFactory_Spread(10.0, 0.0);
			sim.add(spreadTF);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResults res = sim.run();
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
				
			IAgentEvaluation ae = metricEF.create();
			
			aeb.addAgentEvaluation(ae);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "aeb: "+aeb.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "res: "+res.ToStringLong());
			
			Assert.AreEqual(res.Valid, true);
		}
		
		
		[Test()]
		public void Agent0x1Simulation_TrajectoryBundles()
		{
			Console.WriteLine("Agent0x1Simulation_TrajectoryBundles");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Agent0x1), LogLevel.Info);
			//LoggerInitialization.SetThreshold(typeof(AbstractAgent), LogLevel.Info);
			//LoggerInitialization.SetThreshold(typeof(SimulationBundle), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Scheduler), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Trajectory), LogLevel.Debug);
			
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauPoint mean = new BlauPoint(s);
			IBlauPoint std = new BlauPoint(s);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new Agent0x0_Factory(d);
			int NUMAGENTS = 25;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			pop.addAgent(loader);
			
			IOrderbook_Observable ob = new Orderbook();
			
			string PROPERTY = "NetWorth";
			
			// 1 hours
			ISimulationBundle simb = new SimulationBundle(pop, ob, 0.0, 3600.0);

			IAgentEvaluationFactory metricEF = new NamedMetricAgentEvaluationFactory(PROPERTY);
			simb.add (metricEF);
			
			ITrajectoryFactory priceTF = new TrajectoryFactory_Price(10.0, 0.8);
			simb.add(priceTF);
			
			ITrajectoryFactory spreadTF = new TrajectoryFactory_Spread(10.0, 0.0);
			simb.add(spreadTF);
			
			int NUMTRIALS = 25;
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResultsBundle resb = simb.run(NUMTRIALS);
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "resb: "+resb.ToString());
			
			int STEPS = 1;
			int [] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			
			foreach (IAgentEvaluationBundle aeb in resb.getAgentEvaluationBundles()) {
				IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
				IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "meanEval: "+meanEval.ToStringLong());
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "stdEval: "+stdEval.ToStringLong());
			}
			
			foreach (ITrajectoryBundle tb in resb.getTrajectoryBundles()) {
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Computing meanTraj");
				ITrajectory meanTraj = tb.MeanTrajectory;
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Computing stdTraj");
				ITrajectory stdTraj = tb.StdTrajectory;
				
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "meanTraj: "+meanTraj.ToStringLong());
				SingletonLogger.Instance().DebugLog(typeof(sim_tests), "stdTraj: "+stdTraj.ToStringLong());
			}
			
			Assert.AreEqual(resb.Valid, true);
		}
		
		
		[Test()]
		public void Agent0x1Simulation_AgentTrajectories1()
		{
			Console.WriteLine("Agent0x1Simulation_AgentTrajectories1");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Agent0x0), LogLevel.Info);
			
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauPoint mean = new BlauPoint(s);
			IBlauPoint std = new BlauPoint(s);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new Agent0x0_Factory(d);
			int NUMAGENTS = 10;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			pop.addAgent(loader);
			
			IOrderbook_Observable ob = new Orderbook();
			
			// 1 hours
			ISimulation sim = new Simulation(pop, ob, 0.0, 3600.0);
			
			IAgent agent = PopulationSelector.Select(pop);
			ITrajectoryFactory agTF = new TrajectoryFactory_AgentOrders(agent, 10.0, 0.0);
			sim.add(agTF);
			ITrajectoryFactory agTF2 = new TrajectoryFactory_AgentNamedMetric(agent, "NetWorth", 10.0, 0.0);
			sim.add(agTF2);
			ITrajectoryFactory agTF3 = new TrajectoryFactory_AgentNamedMetric(agent, "NetWorth", 10.0, 0.99);
			sim.add(agTF3);
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulation");
			ISimulationResults res = sim.run();
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulation");
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Results\n"+res.ToStringLong());
			Assert.AreEqual(res.Valid, true);
		}
		
		[Test()]
		public void Agent0x1Simulation_AgentTrajectories_MultiTrial()
		{
			Console.WriteLine("Agent0x1Simulation_AgentTrajectories_MultiTrial");
			LoggerInitialization.SetThreshold(typeof(sim_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Agent0x0), LogLevel.Info);
			
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauPoint mean = new BlauPoint(s);
			IBlauPoint std = new BlauPoint(s);
			IDistribution d = new Distribution_Gaussian(s,mean, std);
			
			IAgentFactory afact = new Agent0x0_Factory(d);
			int NUMAGENTS = 10;
			IPopulation pop = PopulationFactory.Instance().create(afact, NUMAGENTS);
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PATH = ""+ApplicationConfig.EXECDIR+"orderbooks/orderbook.csv";
			AgentOrderbookLoader loader = MakeAgentOrderbookLoader(PATH);
			pop.addAgent(loader);
			
			IOrderbook_Observable ob = new Orderbook();
			
			// 1 hours
			ISimulationBundle simb = new SimulationBundle(pop, ob, 0.0, 3600.0);
			
			IAgent agent = PopulationSelector.Select(pop);
			ITrajectoryFactory agTF = new TrajectoryFactory_AgentOrders(agent, 10.0, 0.0);
			simb.add(agTF);
			
			int NUMTIALS = 2;
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Running Simulations");
			ISimulationResultsBundle resb = simb.run(NUMTIALS);
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Stopping Simulations");
			
			SingletonLogger.Instance().DebugLog(typeof(sim_tests), "Results Bundle\n"+resb.ToStringLong());
			
			Assert.AreEqual(resb.Valid, true);
		}
	}
}

