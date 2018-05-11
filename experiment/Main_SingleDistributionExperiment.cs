using System;
using System.IO;
using core;
using logger;
using config;
using presentation;
using models;
using agent;


namespace experiment
{
	public class Main_SingleDistributionExperiment
	{	
		public static void RunExperiment(IExperiment exp)
		{
			LoggerInitialization.SetThreshold(typeof(SingleDistributionExperiment), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Main_SingleDistributionExperiment), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Agent1x0), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(AbstractExperiment), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(AbstractAgent), LogLevel.Debug);
			
			//LoggerInitialization.SetThreshold(typeof(AbstractAgentFactory), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(PopulationFactory), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(SingleDistributionExperiment), LogLevel.Debug);
			
			
			Console.WriteLine("Running simulation...");
			exp.run();
			Console.WriteLine("Completed.");
			
		}
		
		public static void Main (string[] args)
		{
			if (args.Length!=1) {
				string prog = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
				Console.WriteLine ("Usage: "+prog+" [ExecutionDirectoryName]");
				Console.WriteLine ("   or: "+prog+" [NewExperimentName]");
				Environment.Exit(-1);
			}

			// get rid of this for METAEXPERIMENTS, we need it for EXPERIMENTS
			// Directory.SetCurrentDirectory(ApplicationConfig.ORIGINAL_EXECDIR_VALUE);

			string module = ApplicationConfig.InitializeAtCurrentWorkingDirectory();
			Console.WriteLine ("Module name: ["+module+"]");

			string exedir;
			exedir = Path.Combine(ApplicationConfig.EXECDIR, args[0]);
			
			IExperiment exp;
			if ( ! Directory.Exists(exedir)) {
				exp = SingleDistributionExperiment.Create(ApplicationConfig.EXECDIR, args[0]);
			}
			else {
				exp = SingleDistributionExperiment.Load(ApplicationConfig.EXECDIR, args[0]);
			}
			RunExperiment(exp);
		}
	}
}

