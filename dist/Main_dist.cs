using System;
using System.IO;
using logger;
using core;
using dist;
using blau;

namespace dist
{
	public class Main_dist {
		
		public static void run()
		{
			LoggerInitialization.SetThreshold(typeof(Main_dist), LogLevel.Debug);
			
			SingletonLogger.Instance().DebugLog(typeof(Main_dist), "Main_dist");
		}
		
		public static void Main (string[] args)
		{
			run();
		}
	}
}

