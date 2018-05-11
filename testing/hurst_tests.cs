using System;
using NUnit.Framework;
using logger;
using core;
using des;
using signal;

namespace testing
{
	[TestFixture()]
	public class hurst_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		private double WINDOW = 1024.0;
		
		[Test()]
		public void LinearHurstTest()
		{
			Console.WriteLine("LinearHurstTest");
			LoggerInitialization.SetThreshold(typeof(hurst_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(TrajectoryTransformer_Hurst), LogLevel.Debug);
			double m = 0.1;
			double b = 0.0;
			
			ITrajectory traj = new Trajectory("data", 1.0, 0.0, 0.0);
			double INC = 1.0;
			for (double x=0.0; x<WINDOW; x+=INC) {
				
				double NOISE_SCALE = 0.0001;
				double noise = 2.0*NOISE_SCALE*SingletonRandomGenerator.Instance.NextDouble() - NOISE_SCALE;
				
				double y = m*INC + noise;
				
				traj.add(x,y);
			}
			
			ITrajectoryTransformer tx = new TrajectoryTransformer_Hurst(WINDOW, 1.0);
			ITrajectory trajHurst = tx.eval(traj);
			
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), "Hurst of y = "+m+" x + "+b+"\n");
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), ""+trajHurst.ToStringLong());
		}
		
		[Test()]
		public void RandomWalkTest()
		{
			Console.WriteLine("RandomWalkTest");
			LoggerInitialization.SetThreshold(typeof(hurst_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(TrajectoryTransformer_Hurst), LogLevel.Info);
			double y =0.0;
			
			ITrajectory traj = new Trajectory("data", 1.0, 0.0, 0.0);
			for (double x=0.0; x<WINDOW; x+=1.0) {
				double STEP;
				if (SingletonRandomGenerator.Instance.NextDouble() <= 0.5) {
					STEP = 0.1;
				}
				else {
					STEP = -0.1;
				}
				y = y+STEP;
				// traj.add(x,y);
				traj.add (x,STEP);
			}
			
			ITrajectoryTransformer tx = new TrajectoryTransformer_Hurst(WINDOW, 1.0);
			ITrajectory trajHurst = tx.eval(traj);
			
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), "Hurst of RandomWalk\n");
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), ""+trajHurst.ToStringLong());
		}
		
		[Test()]
		public void ZeroTest()
		{
			Console.WriteLine("ZeroTest");
			LoggerInitialization.SetThreshold(typeof(hurst_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(TrajectoryTransformer_Hurst), LogLevel.Info);
			double y =0.0;
			
			ITrajectory traj = new Trajectory("data", 1.0, 0.0, 0.0);
			for (double x=0.0; x<WINDOW; x+=1.0) {
				y = 0.005*SingletonRandomGenerator.Instance.NextDouble();
				traj.add(x,y);
			}
			
			ITrajectoryTransformer tx = new TrajectoryTransformer_Hurst(WINDOW, 1.0);
			ITrajectory trajHurst = tx.eval(traj);
			
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), "Hurst of Zero\n");
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), ""+trajHurst.ToStringLong());
		}
		
		
		[Test()]
		public void SinTest()
		{
			Console.WriteLine("SinTest");
			LoggerInitialization.SetThreshold(typeof(hurst_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(TrajectoryTransformer_Hurst), LogLevel.Info);
			double y =0.0;
			
			ITrajectory traj = new Trajectory("data", 1.0, 0.0, 0.0);
			for (double x=0.0; x<WINDOW; x+=1.0) {
				y = Math.Sin (x/100.0);
				traj.add(x,y);
			}
			
			ITrajectoryTransformer tx = new TrajectoryTransformer_Hurst(WINDOW, 1.0);
			ITrajectory trajHurst = tx.eval(traj);
			
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), "Hurst of Sin\n");
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), ""+trajHurst.ToStringLong());
		}
		
		[Test()]
		public void RandomBinaryTest()
		{
			Console.WriteLine("RandomBinaryTest");
			LoggerInitialization.SetThreshold(typeof(hurst_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(TrajectoryTransformer_Hurst), LogLevel.Debug);
			double y =0.0;
			
			ITrajectory traj = new Trajectory("data", 1.0, 0.0, 0.0);
			
			bool pos = true;
			
			for (double x=0.0; x<WINDOW; x+=1.0) {
				if (pos) {
					y = 1.0;
					traj.add(x,y);
					pos = false;
				}
				else {
					y = -1.0;
					traj.add(x,y);
					pos = true;
				}
			}
			
			ITrajectoryTransformer tx = new TrajectoryTransformer_Hurst(WINDOW, 1.0);
			ITrajectory trajHurst = tx.eval(traj);
			
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), "Hurst of Random Alternating\n");
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), ""+trajHurst.ToStringLong());
		}
		
		
		[Test()]
		public void SqrtTest()
		{
			Console.WriteLine("SqrtTest");
			LoggerInitialization.SetThreshold(typeof(hurst_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(TrajectoryTransformer_Hurst), LogLevel.Info);
			double y =0.0;
			
			ITrajectory traj = new Trajectory("data", 1.0, 0.0, 0.0);
			for (double x=0.0; x<WINDOW; x+=1.0) {
				y = Math.Sqrt (x+1) - Math.Sqrt (x);
				traj.add(x,y);
			}
			
			ITrajectoryTransformer tx = new TrajectoryTransformer_Hurst(WINDOW, 1.0);
			ITrajectory trajHurst = tx.eval(traj);
			
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), "Hurst of Random\n");
			SingletonLogger.Instance().DebugLog(typeof(hurst_tests), ""+trajHurst.ToStringLong());
		}
	}
}

