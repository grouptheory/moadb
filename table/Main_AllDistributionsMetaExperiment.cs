using System;
using System.Diagnostics;
using System.IO;
using core;
using logger;
using config;
using presentation;
using models;
using agent;
using dist;

namespace metaexperiment
{
	public class Main_AllDistributionsMetaExperiment
	{
		public static void RunMetaExperiment(IMetaExperiment exp)
		{
			//LoggerInitialization.SetThreshold(typeof(Distribution_Gaussian), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Main_AllDistributionsMetaExperiment), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(DistributionSpaceIterator), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(Agent0x1), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(AbstractAgentFactory), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(PopulationFactory), LogLevel.Debug);
			//LoggerInitialization.SetThreshold(typeof(SingleDistributionExperiment), LogLevel.Debug);
			
			Console.WriteLine("Running metaexperiment: "+exp.Name);
			exp.run();
			Console.WriteLine("Completed metaexperiment: "+exp.Name);
		}
		
		private static void CreateReport(IExperiment exp) {
			
			Console.WriteLine("Creating report for metaexperiment: "+exp.Name);
			
			try
	  		{
		  		//Set the current directory.
		  		Directory.SetCurrentDirectory(Path.Combine(exp.OutputDirectory, "results"));
	  		}
	  		catch (DirectoryNotFoundException e)
	  		{
		  		Console.WriteLine("The specified results directory "+Path.Combine(ApplicationConfig.EXECDIR, "results")+" does not exist. {0}", e);
	  		}
			
			ProcessStartInfo startInfo = new ProcessStartInfo();

			startInfo.FileName   = ApplicationConfig.ROOTDIR+"bin/report.sh";
            startInfo.Arguments = ""+exp.Name;
				
			try
			{
	    		// Start the process with the info we specified.
	    		// Call WaitForExit and then the using statement will close.
	    		using (Process report = Process.Start(startInfo))
	    		{
					report.WaitForExit();
	   			}
			}
			catch
			{
	    		// Log error.
			}
			
		}
		
		public static void Main (string[] args)
		{
			if (args.Length!=1) {
				string prog = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
				Console.WriteLine ("Usage: "+prog+" [ExecutionDirectoryName]");
				Console.WriteLine ("   or: "+prog+" [NewExperimentName]");
				Environment.Exit(-1);
			}
			
			ApplicationConfig.Initialize();
			
			string exedir;
			exedir = Path.Combine(ApplicationConfig.EXECDIR, args[0]);
			
			IMetaExperiment exp;
			if ( ! Directory.Exists(exedir)) {
				exp = AllDistributionsMetaExperiment.Create(ApplicationConfig.EXECDIR, args[0]);
			}
			else {
				exp = AllDistributionsMetaExperiment.Load(ApplicationConfig.EXECDIR, args[0]);
			}
			RunMetaExperiment(exp);
		}
	}
}

