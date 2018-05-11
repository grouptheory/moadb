using System;
using NUnit.Framework;
using logger;
using core;
using signal;

namespace testing
{
	[TestFixture()]
	public class signal_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		[Test()]
		public void TrajectoryTest ()
		{
			LoggerInitialization.SetThreshold(typeof(signal_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Trajectory), LogLevel.Debug);
			
			Trajectory t = new Trajectory("test trajectory", 0.0, 0.0, 0.0);
			
			Assert.AreEqual(t.Name, "test trajectory");
			t.add(0.0, 2.0);
			SingletonLogger.Instance().DebugLog(typeof(signal_tests), "traj: "+t);
			
			Assert.AreEqual(t.Times.Count, 1);
			Assert.AreEqual(t.eval(-1.0), 2.0);
			Assert.AreEqual(t.eval(+3.0), 2.0);
			Assert.AreEqual(t.MaximumTime, 0.0);
			Assert.AreEqual(t.MinimumTime, 0.0);
			Assert.AreEqual(t.Times.Count, 1);
			
			t.add (2.0, 0.0);
			SingletonLogger.Instance().DebugLog(typeof(signal_tests), "traj: "+t);
			
			Assert.AreEqual(t.Times.Count, 2);
			Assert.AreEqual(t.eval(-1.0), 2.0);
			Assert.AreEqual(t.eval(+3.0), 0.0);
			Assert.AreEqual(t.eval(+1.0), 1.0);
			Assert.AreEqual(t.MaximumTime, 2.0);
			Assert.AreEqual(t.MinimumTime, 0.0);
			Assert.AreEqual(t.Times.Count, 2);
		}
		
		
		[Test()]
		public void TrajectoryBundleTest ()
		{
			TrajectoryBundle tb = new TrajectoryBundle("test trajectory");
			
			Trajectory t1 = new Trajectory("test trajectory", 0.0, 0.0, 0.0);
			t1.add(0.0, 2.0);
			t1.add (2.0, 0.0);
			Assert.AreEqual(t1.Times.Count, 2);
			
			Trajectory t2 = new Trajectory("test trajectory", 0.0, 0.0, 0.0);
			t2.add(-1.0, 4.0);
			t2.add(0.0, 4.0);
			t2.add (1.0, 3.0);
			t2.add (2.0, 2.0);
			t2.add (3.0, 4.0);
			Assert.AreEqual(t2.Times.Count, 5);
			
			tb.addTrajectory(t1);
			tb.addTrajectory(t2);
			
			Assert.AreEqual(tb.Trajectories.Contains(t1), true);
			Assert.AreEqual(tb.Trajectories.Contains(t2), true);
			Assert.AreEqual(tb.Times.Count, 5);
			
			ITrajectory t12mean = tb.MeanTrajectory;
			Assert.AreEqual(t1.eval(-1.0), 2.0);
			Assert.AreEqual(t2.eval(-1.0), 4.0);
			Assert.AreEqual(t1.eval(3.0), 0.0);
			Assert.AreEqual(t2.eval(3.0), 4.0);
			Assert.AreEqual(t12mean.eval(-1.0), 3.0);
			Assert.AreEqual(t12mean.eval(0.0), 3.0);
			Assert.AreEqual(t12mean.eval(1.0), 2.0);
			Assert.AreEqual(t12mean.eval(2.0), 1.0);
			Assert.AreEqual(t12mean.eval(0.0), 3.0);
			Assert.AreEqual(t12mean.eval(3.0), 2.0);
			Assert.AreEqual(t12mean.Times.Count, 5);
			Assert.AreEqual(t12mean.MinimumTime, -1.0);
			Assert.AreEqual(t12mean.MaximumTime, 3.0);
			
			ITrajectory t12std = tb.StdTrajectory;
			Assert.AreEqual(t12std.eval(0.0), t12std.eval(1.0));
			Assert.AreEqual(t12std.eval(1.0), t12std.eval(2.0));
			Assert.AreEqual(t12std.eval(2.0), t12std.eval(0.0));
			Assert.AreEqual(t12std.eval(-1.0), 1.0);
			Assert.AreEqual(t12std.eval(3.0), 2.0);
			Assert.AreEqual(t12std.Times.Count, 5);
			Assert.AreEqual(t12std.MinimumTime, -1.0);
			Assert.AreEqual(t12std.MaximumTime, 3.0);
		}
		
		
		
		[Test()]
		public void TrajectoryBundleCollapser_CentralDTW_Test ()
		{
			TrajectoryBundle tb = new TrajectoryBundle("test trajectory");
			
			Trajectory t1 = new Trajectory("test trajectory", 0.0, 0.0, 0.0);
			double v = 1.0;
			for (double t=0; t<=3600; t+=10) {
				t1.add(t, v);
				v *= -1.0;
			}
			Console.WriteLine("t1 = "+t1.GetHashCode());
			
			Trajectory t2 = new Trajectory("test trajectory", 0.0, 0.0, 0.0);
			v = 2.0;
			for (double t=0; t<=3600; t+=10) {
				t2.add(t+23.0, v);
				v *= -1.0;
			}
			Console.WriteLine("t2 = "+t2.GetHashCode());
			
			Trajectory t3 = new Trajectory("test trajectory", 0.0, 0.0, 0.0);
			v = 0.1;
			for (double t=0; t<=3600; t+=10) {
				t3.add(t+36.0, v);
				v *= -1.0;
			}
			Console.WriteLine("t3 = "+t3.GetHashCode());
			
			tb.addTrajectory(t1);
			tb.addTrajectory(t2);
			tb.addTrajectory(t3);
			
			ITrajectory central = tb.CentralTrajectory;
			
			for (double t=0; t<=3600; t+=10) {
				Assert.AreEqual(central.eval(t), t1.eval(t));
				Assert.AreNotEqual(central.eval(t), t2.eval(t));
				Assert.AreNotEqual(central.eval(t), t3.eval(t));
			}
			
			
			ITrajectory dev = tb.CentralDevTrajectory;
			
			for (double t=0; t<=3600; t+=10) {
				Console.WriteLine("t="+t+" dev="+dev.eval(t));
			}
		}
	}
}

