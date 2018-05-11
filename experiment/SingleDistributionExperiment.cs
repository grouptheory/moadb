using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using logger;
using core;
using blau;
using metrics;
using models;
using dist;
using sim;
using agent;
using orderbook;
using signal;
using config;
using presentation;

namespace experiment
{
	public class SingleDistributionExperiment : AbstractExperiment
	{	
		private static readonly double COMMON_BURNIN_TIME_SECONDS = 1.0;
		private static bool LOG_FRAMES = false;

		public static string MakeName(string root, string expname, bool date) {
			DateTime theDate = DateTime.Now;
			string ts = date ? theDate.ToString("yyyyMMdd-hhmmss") : "";
			string fullname = ""+EXPERIMENT_PREFIX+"."+expname+"."+ts;
			return fullname;
		}

		public static SingleDistributionExperiment Create(string root, string expname, bool date=true) {
			string fullname = MakeName(root, expname, date);
			return new SingleDistributionExperiment(root, expname, fullname, true);
		}
		
		public static SingleDistributionExperiment Load(string root, string fullname) {
			int dot = fullname.IndexOf(".");
			string tail = fullname.Substring(dot+1);
			dot = tail.LastIndexOf(".");
			string expname = tail.Substring(0,dot);
			return new SingleDistributionExperiment(root, expname, fullname, false);
		}
	
		private SingleDistributionExperiment(string root, string expname, string fullname, bool create) 
		: base(root, expname, fullname, create) {
		}

		private void savePopulation (string poppath, IPopulation pop)
		{
			TextWriter popWriter = File.CreateText (poppath);
			popWriter.Write ("# ID \t BlauPoint-Coordinates");
			popWriter.WriteLine ("");
			
			foreach (IAgent ag in pop) {
				popWriter.Write ("" + ag.ID + "\t");
				for (int c=0; c<ag.Coordinates.Space.Dimension; c++) {
					popWriter.Write ("" + ag.Coordinates.Space.getAxis (c).Name + "=" + ag.Coordinates.getCoordinate (c) + "\t");
				}
				popWriter.WriteLine ("");
			}
			popWriter.Flush ();
			popWriter.Close ();
		}

		public override void run()
		{
			LoggerInitialization.SetThreshold(typeof(SingleDistributionExperiment), LogLevel.Debug);

			SingletonLogger.Instance().DebugLog(typeof(SingleDistributionExperiment), "run()");
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "distribution => "+theActualDistribution);

			IAgentFactory af = CreateAgentFactory(theActualDistribution, theTableConfig.AgentFactoryClassName);
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "agent factory => "+theTableConfig.AgentFactoryClassName);
			
			af.Initialize (theTableConfig);
			
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "creating initial orderbook");
			IOrderbook_Observable ob = new Orderbook();

			/* No Burnin at this point
			 * 
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Executing burnin...");
			IPopulation popburn = CreatePopulation(af, theTableConfig.NumAgents, Path.Combine(ApplicationConfig.ROOTDIR, theTableConfig.InitialOrderbook));
			ISimulation simburn = new Simulation(popburn, ob, 0.0, COMMON_BURNIN_TIME_SECONDS, false, "burnin") ;
			ISimulationResults resultsburn = simburn.run();
			ob = (IOrderbook_Observable)simburn.Orderbook;
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Done with burnin.");
			*/

			ISimulationBundle accumulated_simb = null;
			ISimulationResultsBundle accumulated_resb = null;	
			
			for (int popi=0; popi<theTableConfig.Populations; popi++) {
				SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "creating population... "+theTableConfig.NumAgents);
				// Populations all contain an OrderBookLoader agent
				IPopulation pop = CreatePopulation(af, theTableConfig.NumAgents, Path.Combine(ApplicationConfig.ROOTDIR, theTableConfig.InitialOrderbook));
				// SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "done.");
					
				string popname = POP_FILE_PREFIX+"."+popi+".Validation";
				string poppath = Path.Combine(_popdir, popname);
				SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "population "+popi+" => "+poppath);
				savePopulation(poppath, pop);
						
				_simb = new SimulationBundle(pop, ob, 0.0, theTableConfig.DurationHours*3600.0, LOG_FRAMES, "pop-"+popi);
		
				if (accumulated_simb == null) {
					accumulated_simb = _simb;
				}
		
				foreach (TrajectoryFactoryConfig tfc in theTrajConfig.getTrajectories()) {
					IPassiveTrajectoryFactory tf = CreatePassiveTrajectoryFactory(tfc);
					tf.Initialize(theTableConfig);
					_simb.add(tf);
				}

				/*
				foreach (IAgent ag in pop) {
					if (ag is IAgent_NonParticipant) continue;

					_simb.add (new TrajectoryFactory_AgentNamedMetric(ag, Agent1x0.NetWorth_METRICNAME, 1.0, 0.0));
				}
*/

				double MAX_ALPHA = 0.10;
				double TEMPORAL_GRANULARITY_FOR_ALPHA_SLICES = 1.0;

				for (double alpha = 0.0; alpha <= MAX_ALPHA; alpha += MAX_ALPHA/(double)theTableConfig.NumCombs) {
					
					TrajectoryFactory_AlphaSlice tf = new TrajectoryFactory_AlphaSlice(TEMPORAL_GRANULARITY_FOR_ALPHA_SLICES, 0.0, alpha, true);
					tf.Initialize(theTableConfig);
					_simb.add(tf);
					TrajectoryFactory_AlphaSlice tf2 = new TrajectoryFactory_AlphaSlice(TEMPORAL_GRANULARITY_FOR_ALPHA_SLICES, 0.0, alpha, false);
					tf2.Initialize(theTableConfig);
					_simb.add(tf2);
				}
				
				foreach (AgentEvaluationConfig aefc in theAgentEvaluationFactorySetConfig.getAgentEvaluations()) {
					IAgentEvaluationFactory aef = CreateAgentEvaluationFactory(aefc);
					_simb.add(aef, aefc);
				}
				
				_resb = _simb.run(theTableConfig.Trials);
		
				//popname = POP_FILE_PREFIX+"."+popi+".Validation-Post";
				//poppath = Path.Combine(_popdir, popname);
				//SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "population post "+popi+" => "+poppath);
				//savePopulation(poppath, pop);

				if (accumulated_resb == null) {
					accumulated_resb = _resb;
				}
				else {
					accumulated_resb.add (_resb);
				}
				
				//WriteTrajectories(popi, _simb, _resb);
			}
			
			//WriteTrajectories(-1, accumulated_simb, accumulated_resb);
			
			PresentationConfig.Directory = Path.Combine(OutputDirectory,"results");
			if ( ! Directory.Exists(PresentationConfig.Directory)) {
				Directory.CreateDirectory(PresentationConfig.Directory);
			}

			Latex.ClearImages();
			
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Writing BlauSpaceEvaluations.");
			
			foreach (IAgentEvaluationBundle aeb in accumulated_resb.getAgentEvaluationBundles()) {
				IBlauSpaceLattice bsl = accumulated_simb.getLattice (theActualDistribution, aeb);
				IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
				IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
					
				BlauSpaceEvaluationGnuplotPresenter pres = new BlauSpaceEvaluationGnuplotPresenter(_bsedir,PresentationConfig.Directory);
				pres.Present(this, meanEval, stdEval);
			}
			
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Writing TrajectoryBundles.");
			foreach (ITrajectoryBundle tb in accumulated_resb.getTrajectoryBundles()) {
				TrajectoryBundleGnuplotPresenter pres = new TrajectoryBundleGnuplotPresenter(_trajdir,PresentationConfig.Directory);
				pres.Present(this, tb);
			}
			
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Writing Latex.");
			Latex.Instance().Present(this);
			
			CreateReport(this);
			
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Done.");
		}
		
		private void WriteTrajectories(int popi, ISimulationBundle simb, ISimulationResultsBundle resb) {
			
			string trajname = TRAJ_FILE_PREFIX+"."+popi+".Validation";
			string trajpath = Path.Combine(_trajdir, trajname);
			TextWriter trajWriter = File.CreateText(trajpath);
					
			foreach (IAgentEvaluationBundle aeb in resb.getAgentEvaluationBundles()) {
				trajWriter.WriteLine("# AgentEvaluation: "+aeb.Name);
				trajWriter.Write("# BlauPoint-Coordinates \t ");
				trajWriter.Write("mean \t std \t count");
				trajWriter.WriteLine("");
				
				IBlauSpaceLattice bsl =  simb.getLattice(theActualDistribution, aeb);
				
				IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
				IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
				IBlauSpaceEvaluation ctEval = aeb.AssignmentCounts(bsl);
				
				foreach (IBlauPoint bp in meanEval.AssignedLatticePoints) {
					for (int c=0;c<bp.Space.Dimension;c++) {
						trajWriter.Write(""+bp.Space.getAxis(c).Name+"="+bp.getCoordinate(c).ToString("#0.000") +"\t");
					}
					trajWriter.Write(""+meanEval.eval (bp).ToString("#0.000") +"\t");
					trajWriter.Write(""+stdEval.eval (bp).ToString("#0.000") +"\t");
					trajWriter.Write(""+ctEval.eval (bp).ToString("#0.000") +"\t");
					trajWriter.WriteLine("");
				}
			}

			double NUMTICKS = 100.0;
			foreach (ITrajectoryBundle tb in resb.getTrajectoryBundles()) {
				trajWriter.WriteLine("# TrajectoryBundle: "+tb.Name);
				trajWriter.WriteLine("# time \t mean \t std \t center \t dev");
				
				ITrajectory meanTraj = tb.MeanTrajectory;
				ITrajectory stdTraj = tb.StdTrajectory;
				ITrajectory centerTraj = tb.CentralTrajectory;
				ITrajectory devTraj = tb.CentralDevTrajectory;
				
				double mint = tb.MinimumTime;
				double maxt = tb.MaximumTime;
				double stept = (maxt - mint)/NUMTICKS;

				for (double t=mint; t<maxt; t+=stept) {
					trajWriter.Write(""+t.ToString("#0.000") +"\t");
					trajWriter.Write(""+meanTraj.eval (t).ToString("#0.000") +"\t");
					trajWriter.Write(""+stdTraj.eval (t).ToString("#0.000") +"\t");
					trajWriter.Write(""+centerTraj.eval (t).ToString("#0.000") +"\t");
					trajWriter.Write(""+devTraj.eval (t).ToString("#0.000") +"\t");
					trajWriter.WriteLine("");
				}
			}
			
			trajWriter.Flush ();
			trajWriter.Close ();
		}
		
		private static void CreateReport(IExperiment exp) {
			
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Creating Report for experiment: "+exp.Name);
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
					SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "evince "+Directory.GetCurrentDirectory()+"/report-*.pdf");
					report.WaitForExit();
	   			}
			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR in CreateReport."+e);
	    		// Log error.
			}
			
			SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "Creating Report Done.");
		}
	}
}

