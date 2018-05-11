using System;
using System.Collections.Generic;
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

namespace testing
{
	[TestFixture()]
	public class metrics_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(metrics_tests), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(AgentDummy), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
	
		void setCoordinateBadly1() {
		}
		
		[Test()]
		public void BlauPointComparerTest()
		{
			Console.WriteLine("BlauPointComparerTest");
			

			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			IBlauPoint bp = new BlauPoint(s);
			bp.setCoordinate(0, 10.0);
			bp.setCoordinate(1, 20.0);
			bp.setCoordinate(2, 30.0);
			IBlauPoint bp2 = bp.clone();
			
			Dictionary<IBlauPoint, int> dic = new Dictionary<IBlauPoint, int>(new BlauPointComparer());
			dic.Add(bp, 1);
			Assert.AreEqual(dic.ContainsKey(bp), true);
			Assert.AreEqual(dic.ContainsKey(bp2), true);
			Assert.Throws<System.ArgumentException>( delegate { dic.Add(bp2, 2); });
			Assert.AreEqual(dic.Count, 1);
			Assert.AreEqual(dic[bp], 1);
			Assert.AreEqual(dic[bp2], 1);
			
			Dictionary<IBlauPoint, int> dic2 = new Dictionary<IBlauPoint, int>(new BlauPointComparer());
			dic2.Add(bp2, 2);
			Assert.AreEqual(dic2.ContainsKey(bp2), true);
			Assert.AreEqual(dic2.ContainsKey(bp), true);
			Assert.Throws<System.ArgumentException>( delegate { dic.Add (bp, 1); });
			Assert.AreEqual(dic2.Count, 1);
			Assert.AreEqual(dic2[bp], 2);
			Assert.AreEqual(dic2[bp2], 2);
		}
		
		[Test()]
		public void ScoreTest()
		{
			Console.WriteLine("ScoreTest");

			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauPoint bp = new BlauPoint(s);
			bp.setCoordinate(0, 11.0);
			bp.setCoordinate(1, 22.0);
			bp.setCoordinate(2, 33.0);
			IScore score = new Score(bp, 5.0);
			Assert.AreEqual(score.Coordinates.CompareTo(bp), 0);
			Assert.AreEqual(score.Value, 5.0);
		}
		
		[Test()]
		public void BlauSpaceMultiEvaluationTest()
		{
			Console.WriteLine("BlauSpaceMultiEvaluationTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			int STEPS = 10;
			int [] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			IBlauSpaceMultiEvaluation mev = new BlauSpaceMultiEvaluation("net worth", bsl);
			
			IBlauPoint bp = new BlauPoint(s);
			bp.setCoordinate(0, 11.0);
			bp.setCoordinate(1, 22.0);
			bp.setCoordinate(2, 33.0);
			
			mev.set(bp, 10.0);
			Assert.AreEqual(mev.eval(bp).Count, 1);
			mev.set(bp, 20.0);
			Assert.AreEqual(mev.eval(bp).Count, 2);
			
			IBlauPoint bp2 = bp.clone();
			mev.set(bp2, 30.0);
			Assert.AreEqual(mev.eval(bp).Count, 3);
			Assert.AreEqual(mev.eval(bp2).Count, 3);
			mev.set(bp2, 40.0);
			Assert.AreEqual(mev.eval(bp).Count, 4);
			Assert.AreEqual(mev.eval(bp2).Count, 4);
			
			Assert.AreEqual(mev.AssignedLatticePoints.Count, 1);
			
			IBlauPoint bp3 = new BlauPoint(s);
			bp3.setCoordinate(0, 12.0);
			bp3.setCoordinate(1, 23.0);
			bp3.setCoordinate(2, 34.0);
			mev.set(bp3, 50.0);
			Assert.AreEqual(mev.eval(bp).Count, 5);
			Assert.AreEqual(mev.eval(bp2).Count, 5);
			Assert.AreEqual(mev.eval(bp3).Count, 5);
			Assert.AreEqual(mev.AssignedLatticePoints.Count, 1);
			
			IBlauPoint bpX = new BlauPoint(s);
			bpX.setCoordinate(0, 22.0);
			bpX.setCoordinate(1, 33.0);
			bpX.setCoordinate(2, 44.0);
			mev.set(bpX, 100.0);
			Assert.AreEqual(mev.eval(bp).Count, 5);
			Assert.AreEqual(mev.eval(bp2).Count, 5);
			Assert.AreEqual(mev.eval(bp3).Count, 5);
			Assert.AreEqual(mev.eval(bpX).Count, 1);
			Assert.AreEqual(mev.AssignedLatticePoints.Count, 2);
			
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "mev: "+mev.ToStringLong());
		}
		
		[Test()]
		public void BlauSpaceEvaluationTest()
		{
			Console.WriteLine("BlauSpaceEvaluationTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			int STEPS = 10;
			int [] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			IBlauSpaceMultiEvaluation mev = new BlauSpaceMultiEvaluation("net worth", bsl);
			
			IBlauPoint bp = new BlauPoint(s);
			bp.setCoordinate(0, 11.0);
			bp.setCoordinate(1, 22.0);
			bp.setCoordinate(2, 33.0);
			
			mev.set(bp, 10.0);
			mev.set(bp, 20.0);
			IBlauPoint bp2 = bp.clone();
			mev.set(bp2, 30.0);
			mev.set(bp2, 40.0);
			IBlauPoint bp3 = new BlauPoint(s);
			bp3.setCoordinate(0, 12.0);
			bp3.setCoordinate(1, 23.0);
			bp3.setCoordinate(2, 34.0);
			mev.set(bp3, 50.0);
			IBlauPoint bpX = new BlauPoint(s);
			bpX.setCoordinate(0, 22.0);
			bpX.setCoordinate(1, 33.0);
			bpX.setCoordinate(2, 44.0);
			mev.set(bpX, 100.0);
			
			IBlauSpaceEvaluation bse = new BlauSpaceEvaluation("net worth", bsl);
			
			foreach (IBlauPoint p in mev.AssignedLatticePoints) {
				LinkedList<IScore> scores = mev.eval (p);
				double total = 0.0;
				double count = 0.0;
				foreach (IScore sc in scores) {
					total += sc.Value;
					count += 1.0;
				}
				if (p.CompareTo(bp)==0) {
					Assert.AreEqual(total, 150.0);
					Assert.AreEqual(count, 5.0);
				}
				if (p.CompareTo(bpX)==0) {
					Assert.AreEqual(total, 100.0);
					Assert.AreEqual(count, 1.0);
				}
				double ave = total/count;
				bse.set(p, ave);
				Assert.Throws<Exception>( delegate { bse.set(p, ave); });
				Assert.Throws<Exception>( delegate { bse.set(p.clone(), ave); });
				
				Assert.AreEqual(bse.eval(p), ave);
				Assert.AreEqual(bse.eval(p.clone()), ave);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "bse: "+bse.ToStringLong());
		}
		
		[Test()]
		public void AgentEvaluationTest()
		{
			Console.WriteLine("AgentEvaluationTest");
			
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			string PROPERTY = "NetWorth";
			IAgentEvaluation ae = new AgentEvaluation(PROPERTY, null);
			
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
			IAgent[] agents = new IAgent[NUMAGENTS];
			
			for (int i=0; i<NUMAGENTS; i++) {
				agents[i] = afact.create();
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent["+i+"]: "+agents[i]);
			}
			
			double VAL = 1.0;
			
			for (int i=0; i<NUMAGENTS; i++) {
				ae.set(agents[i], VAL);
			}
			
			for (int i=0; i<NUMAGENTS; i++) {
				Assert.AreEqual(ae.eval(agents[i]), VAL);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "ae: "+ae.ToStringLong());
		}
		
		
		[Test()]
		public void BlauSpaceMultiEvaluationConstructionTest()
		{
			Console.WriteLine("BlauSpaceMultiEvaluationConstructionTest");
			
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			int STEPS = 10;
			int [] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			
			
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
			IAgent[] agents = new IAgent[NUMAGENTS];
			for (int i=0; i<NUMAGENTS; i++) {
				agents[i] = afact.create();
			}
			
			string PROPERTY = "NetWorth";
			
			BlauSpaceMultiEvaluation mev = new BlauSpaceMultiEvaluation(PROPERTY, bsl);
			
			int NUMTRIALS = 10;
			for (int trial=0; trial<NUMTRIALS; trial++) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "*** Trial "+trial);
				IAgentEvaluation ae = new AgentEvaluation(PROPERTY, null);
				for (int i=0; i<NUMAGENTS; i++) {
					ae.set(agents[i], SingletonRandomGenerator.Instance.NextGaussian(1.0, 0.2));
				}
			
				ae.AddToBlauSpaceMultiEvaluation(mev);
				
				int count = 0;
				foreach (IBlauPoint p in mev.AssignedLatticePoints) {
					LinkedList<IScore> scores = mev.eval(p);
					SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "" + scores.Count + " Readings binned to Blaupoint: "+p);
					count += scores.Count;
				}
				Assert.AreEqual(count, NUMAGENTS*(trial+1));
			}
			
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "mev: "+mev.ToStringLong());
		}
		
		[Test()]
		public void AgentEvaluationBundleCollapsingTest()
		{
			Console.WriteLine("AgentEvaluationBundleCollapsingTest");
			
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			int STEPS = 10;
			int [] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			
			
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
			IAgent[] agents = new IAgent[NUMAGENTS];
			for (int i=0; i<NUMAGENTS; i++) {
				agents[i] = afact.create();
			}
			
			string PROPERTY = "NetWorth";
			
			IAgentEvaluationBundle aeb = new AgentEvaluationBundle(PROPERTY);
			
			int NUMTRIALS = 1000;
			double MEANVAL = 1.0;
			
			for (int trial=0; trial<NUMTRIALS; trial++) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "*** Trial "+trial);
				IAgentEvaluation ae = new AgentEvaluation(PROPERTY, null);
				for (int i=0; i<NUMAGENTS; i++) {
					ae.set(agents[i], SingletonRandomGenerator.Instance.NextGaussian(MEANVAL, 0.2));
				}
			
				aeb.addAgentEvaluation(ae);
			}
			
			IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
			IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
			
			foreach (IBlauPoint p in meanEval.AssignedLatticePoints) {
				double meanval = meanEval.eval(p);
				double stdval = stdEval.eval(p);
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "Scores binned to Blaupoint: "+p+" ===> mean:"+meanval+", std:"+stdval);
			
				Assert.Less(Math.Abs(meanval - MEANVAL), 0.1);
				Assert.Less(stdval, 0.1);
			}

			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "aeb: "+aeb.ToString());
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "meanEval: "+meanEval.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "stdEval: "+stdEval.ToStringLong());
		}
		
		[Test()]
		public void NamedMetricAgentEvaluationFactoryTest()
		{
			Console.WriteLine("NamedMetricAgentEvaluationFactoryTest");
			
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			int STEPS = 10;
			int [] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			
			
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
			IOrderbook_Observable ob = new Orderbook();
			
			foreach (IAgent ag in pop) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "agent: "+ag);
			}
			
			string PROPERTY = "NetWorth";
			
			IAgentEvaluationBundle aeb = new AgentEvaluationBundle(PROPERTY);
			
			int NUMTRIALS = 100;

			for (int trial=0; trial<NUMTRIALS; trial++) {
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "*** Trial "+trial);
				
				ISimulation sim = new Simulation(pop.clone(), ob.clone(), 0.0, 100.0);
				NamedMetricAgentEvaluationFactory metricEF = new NamedMetricAgentEvaluationFactory(PROPERTY);
				sim.add (metricEF);
				sim.broadcast(new SimulationStart());
				sim.broadcast(new SimulationEnd());
				
				IAgentEvaluation ae = metricEF.create();
			
				aeb.addAgentEvaluation(ae);
			}
			
			IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
			IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
			
			foreach (IBlauPoint p in meanEval.AssignedLatticePoints) {
				double meanval = meanEval.eval(p);
				double stdval = stdEval.eval(p);
				SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "Scores binned to Blaupoint: "+p+" ===> mean:"+meanval+", std:"+stdval);
			
				Assert.Less(Math.Abs(meanval - AgentDummy.MEANWORTH), 0.1);
				Assert.Less(stdval, 0.1);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "aeb: "+aeb.ToString());
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "meanEval: "+meanEval.ToStringLong());
			SingletonLogger.Instance().DebugLog(typeof(metrics_tests), "stdEval: "+stdEval.ToStringLong());
		}
	}
}

