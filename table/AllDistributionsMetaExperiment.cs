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
using experiment;
using presentation;

namespace metaexperiment
{
	public class AllDistributionsMetaExperiment : AbstractMetaExperiment
	{		
		public static string MakeName(string root, string expname, bool date) {
			DateTime theDate = DateTime.Now;
			string ts = date ? theDate.ToString("yyyyMMdd-hhmmss") : "";
			string fullname = ""+METAEXPERIMENT_PREFIX+"."+expname+"."+ts;
			return fullname;
		}

		public static AllDistributionsMetaExperiment Create(string root, string expname, bool date=true) {
			string fullname = MakeName(root, expname, date);
			return new AllDistributionsMetaExperiment(root, expname, fullname, true);
		}
		
		public static AllDistributionsMetaExperiment Load(string root, string fullname) {
			int dot = fullname.IndexOf(".");
			string tail = fullname.Substring(dot+1);
			dot = tail.LastIndexOf(".");
			string expname = tail.Substring(0,dot);
			return new AllDistributionsMetaExperiment(root, expname, fullname, false);
		}

		private AllDistributionsMetaExperiment(string root, string expname, string fullname, bool create) 
		: base(root, expname, fullname, create) {
		}
		
		public override void run()
		{
			SingletonLogger.Instance().DebugLog(typeof(AllDistributionsMetaExperiment), "AllDistributionsMetaExperiment.run()");
			
			int [] steps = theDistributionSpaceIteratorSpecification.ApplyToDistribution(this.theTemplateDistribution);
			DistributionSpace ds = new DistributionSpace(this.theTemplateDistribution);
			
			IDistributionSpaceIterator it = ds.iterator(steps);
    		
			using (TextWriter writer = File.CreateText(Path.Combine(_distdir, "STATUS")))
			{
			    writer.WriteLine("Running");
			}
			
			TextWriter indexWriter = File.CreateText(Path.Combine(_distdir, "INDEX"));
			
			SoapFormatter formatter = new SoapFormatter();
			
			Console.WriteLine("Generating distributions...");

			int count = 0;
			int validCt = 0;
			foreach (IDistribution d2 in it) {
				bool valid = d2.IsValid();
				if ( valid ) {
					// SingletonLogger.Instance().DebugLog(typeof(Main_DistributionEnumerator), "iterator distribution: "+d2);
					
					string distfilename = ""+DIST_FILE_PREFIX+"."+validCt.ToString("D6");
					string distfilepath = Path.Combine(_distdir, distfilename);
					
					FileStream fs2 = new FileStream(distfilepath, FileMode.Create);
					formatter.Serialize(fs2, d2);
    				fs2.Close();

					if (validCt==0) {
						indexWriter.Write("XXXXXX #\tfile\t");
						for (int j=0; j<d2.SampleSpace.Dimension; j++) {
							indexWriter.Write(d2.SampleSpace.getAxis(j).Name+"\t");
						}
						indexWriter.WriteLine();
					}

					//indexWriter.WriteLine(""+validCt+"\t"+distfilename+"\t"+d2.ToString());
					string distAsString = "";
					for (int i=0; i<d2.Params; i++) {
						string paramAsString = d2.getParamName(i)+"="+d2.getParam(i);
						distAsString += paramAsString + "\t";
					}

					indexWriter.WriteLine("XXXXXX "+validCt+"\t"+distfilename+"\t"+distAsString);

					// Console.WriteLine("Generated distribution: "+d2);
					
					validCt++;
				}
				else {
					// Console.WriteLine("Invalid distribution: "+d2);
				}
				count++;
			}
			
			indexWriter.Flush();
			indexWriter.Close();
			
			using (TextWriter writer = File.CreateText(Path.Combine(_distdir, "STATUS")))
			{
			    writer.WriteLine("Finished");
			}
			Console.WriteLine("Done! valid distributions: "+validCt+" / total: "+count);
		
			ApplicationConfig.Initialize(_outdir);

			// reset
			ds = new DistributionSpace(this.theTemplateDistribution);
			it = ds.iterator(steps);
			int count2 = 0;
			int validCt2 = 0;
			foreach (IDistribution d2 in it) {
				if ( d2.IsValid() ) {
					string distnum = ""+validCt2.ToString("D6");
					string exedir = SingleDistributionExperiment.MakeName(_outdir, distnum, false);

					string distfilename = ""+DIST_FILE_PREFIX+"."+validCt2.ToString("D6");
					string distfilepath = Path.Combine(_distdir, distfilename);
					File.Copy(distfilepath, Path.Combine(_outdir, AbstractExperiment.ACTUAL_DIST_FILE),true);
					
					Console.WriteLine("Preparing simulation: "+validCt2+" / total: "+validCt);

					IExperiment exp;
					if ( ! Directory.Exists(exedir)) {
						exp = SingleDistributionExperiment.Create(_outdir, distnum, false);
					}
					else {
						exp = SingleDistributionExperiment.Load(_outdir, distnum);
					}

					/*
					Console.WriteLine("exedir: "+exedir);
					Console.WriteLine("root: "+_outdir);

					try {
						Main_SingleDistributionExperiment.RunExperiment(exp);
						Console.WriteLine("...OK");
					}
					catch (Exception ex) {
						Console.WriteLine("...ERROR");
						throw ex;
					}
*/

					validCt2++;
				}
				count2++;
			}


			/*		
			string [] fileEntries = Directory.GetFiles(_distdir);
			
    		foreach(string filepath in fileEntries)
    		{
				string fileName = Path.GetFileName(filepath);
				// SingletonLogger.Instance().InfoLog(typeof(Main_table), "fileName: "+fileName);
				if ( ! fileName.StartsWith (DIST_FILE_PREFIX)) continue;
				
				Console.WriteLine(" ");
				Console.WriteLine("****************************************************");
				Console.WriteLine("Reading distribution: "+filepath);
				Console.WriteLine(" ");
			
				FileStream fs = new FileStream(filepath, FileMode.Open);
				IDistribution  d2 = (IDistribution)formatter.Deserialize(fs);
				fs.Close();
				
				theActualDistribution = d2;
				
				Console.WriteLine("Distribution => "+d2);
				
				IAgentFactory af = CreateAgentFactory(d2, theTableConfig.AgentFactoryClassName);
				
				af.Initialize (theTableConfig);
				
				IOrderbook_Observable ob = new Orderbook();
				
				ISimulationBundle accumulated_simb = null;
				ISimulationResultsBundle accumulated_resb = null;
					
				for (int popi=0; popi<theTableConfig.Populations; popi++) {
					IPopulation pop = CreatePopulation(af, theTableConfig.NumAgents, theTableConfig.InitialOrderbook);
				
					string popname = POP_FILE_PREFIX+fileName.Substring(DIST_FILE_PREFIX.Length)+"."+popi;
					string poppath = Path.Combine(_popdir, popname);
					
					SingletonLogger.Instance().InfoLog(typeof(TableGenerationExperiment), "Population: "+popi);
					
					TextWriter popWriter = File.CreateText(poppath);
					foreach (IAgent ag in pop) {
						popWriter.Write(""+ag.ID+"\t");
						for (int c=0;c<ag.Coordinates.Space.Dimension;c++) {
							popWriter.Write(""+ag.Coordinates.getCoordinate(c)+"\t");
						}
						popWriter.WriteLine("");
					}
					popWriter.Flush ();
					popWriter.Close ();
					
					
					for (int triali=0; triali<theTableConfig.Trials; triali++) {
						
						Console.WriteLine("Population: "+popi+", Running Trial "+triali);
						
						SimulationBundle simb = new SimulationBundle(pop, ob, 0.0, theTableConfig.DurationHours*3600.0);
						
						if (accumulated_simb == null) {
							accumulated_simb = simb;
						}
						
						foreach (TrajectoryFactoryConfig tfc in theTrajConfig.getTrajectories()) {
							IPassiveTrajectoryFactory tf = CreatePassiveTrajectoryFactory(tfc);
							tf.Initialize(theTableConfig);
							simb.add(tf);
						}
								
						double MAX_ALPHA = 0.25;
						for (double alpha = 0.0; alpha <= MAX_ALPHA; alpha += MAX_ALPHA/(double)theTableConfig.NumCombs) {
							TrajectoryFactory_AlphaSlice tf = new TrajectoryFactory_AlphaSlice(0.0, 0.0, alpha, true);
							tf.Initialize(theTableConfig);
							simb.add(tf);
							TrajectoryFactory_AlphaSlice tf2 = new TrajectoryFactory_AlphaSlice(0.0, 0.0, alpha, false);
							tf2.Initialize(theTableConfig);
							simb.add(tf2);
						}
						
						foreach (AgentEvaluationConfig aefc in theAgentEvaluationFactorySetConfig.getAgentEvaluations()) {
							IAgentEvaluationFactory aef = CreateAgentEvaluationFactory(aefc);
							simb.add(aef, aefc);
						}
			
						// run ONE trial
						ISimulationResultsBundle resb = simb.run(1);
						
						if (accumulated_resb == null) {
							accumulated_resb = resb;
						}
						else {
							accumulated_resb.add (resb);
						}
						
						foreach (IAgentEvaluationBundle aeb in resb.getAgentEvaluationBundles()) {
							
							string trajname = TRAJ_FILE_PREFIX+fileName.Substring(DIST_FILE_PREFIX.Length)+"."+popi+"."+triali+".evaluation."+aeb.Name;
							string trajpath = Path.Combine(_trajdir, trajname);
							TextWriter trajWriter = File.CreateText(trajpath);
							
							trajWriter.WriteLine("# AgentEvaluation: "+aeb.Name);
							trajWriter.WriteLine("# blaupoint mean std count");
							IAgentEvaluationFactory aef = null;;
							foreach (IAgentEvaluation iae in aeb.Evaluations) {
								aef = iae.Creator;
								break;
							}
							
							steps = new int[theTemplateDistribution.SampleSpace.Dimension]; 
							for (int j=0;j<theTemplateDistribution.SampleSpace.Dimension;j++) steps[j]=simb.getAgentEvaluationConfig(aef).BlauSpaceGridding;
							
							IBlauSpaceLattice bsl = BlauSpaceLattice.create(theTemplateDistribution.SampleSpace, steps);
							IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
							IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
							IBlauSpaceEvaluation ctEval = aeb.AssignmentCounts(bsl);
							
							foreach (IBlauPoint bp in meanEval.AssignedLatticePoints) {
								for (int c=0;c<bp.Space.Dimension;c++) {
									trajWriter.Write(""+bp.getCoordinate(c)+"\t");
								}
								trajWriter.Write(""+meanEval.eval (bp)+"\t");
								trajWriter.Write(""+stdEval.eval (bp)+"\t");
								trajWriter.Write(""+ctEval.eval (bp)+"\t");
								trajWriter.WriteLine("");
							}
							
							trajWriter.Flush ();
							trajWriter.Close ();
						}
						
						foreach (ITrajectoryBundle tb in resb.getTrajectoryBundles()) {
							string trajname = TRAJ_FILE_PREFIX+fileName.Substring(DIST_FILE_PREFIX.Length)+"."+popi+"."+triali+".trajectory."+tb.Name;
							string trajpath = Path.Combine(_trajdir, trajname);
							TextWriter trajWriter = File.CreateText(trajpath);
							
							trajWriter.WriteLine("# TrajectoryBundle: "+tb.Name);
							trajWriter.WriteLine("# time mean std center dev");
							ITrajectory meanTraj = tb.MeanTrajectory;
							ITrajectory stdTraj = tb.StdTrajectory;
							ITrajectory centerTraj = tb.CentralTrajectory;
							ITrajectory devTraj = tb.CentralDevTrajectory;
		
							foreach (double t in meanTraj.Times) {
								trajWriter.Write(""+meanTraj.eval (t)+"\t");
								trajWriter.Write(""+stdTraj.eval (t)+"\t");
								trajWriter.Write(""+centerTraj.eval (t)+"\t");
								trajWriter.Write(""+devTraj.eval (t)+"\t");
								trajWriter.WriteLine("");
							}
							
							trajWriter.Flush ();
							trajWriter.Close ();
						}
						
					}// trials
				}// populations
				
				
				string resultsname = RESULTS_DIR_PREFIX+fileName.Substring(DIST_FILE_PREFIX.Length);
				PresentationConfig.Directory = Path.Combine(this.OutputDirectory,resultsname);
				if ( ! Directory.Exists(PresentationConfig.Directory)) {
					Directory.CreateDirectory(PresentationConfig.Directory);
				}
				
				Latex.ClearImages();
				
				foreach (IAgentEvaluationBundle aeb in accumulated_resb.getAgentEvaluationBundles()) {
					
					string trajname = TRAJ_FILE_PREFIX+fileName.Substring(DIST_FILE_PREFIX.Length)+".ALL.evaluation."+aeb.Name;
					string trajpath = Path.Combine(_trajdir, trajname);
					TextWriter trajWriter = File.CreateText(trajpath);
					
					Console.WriteLine("Writing AgentEvaluation: "+aeb.Name);
					
					trajWriter.WriteLine("# AgentEvaluation: "+aeb.Name);
					trajWriter.WriteLine("# blaupoint mean std count");
					IAgentEvaluationFactory aef = null;;
					foreach (IAgentEvaluation iae in aeb.Evaluations) {
						aef = iae.Creator;
						break;
					}
					
					steps = new int[theTemplateDistribution.SampleSpace.Dimension]; 
					for (int j=0;j<theTemplateDistribution.SampleSpace.Dimension;j++) steps[j]=accumulated_simb.getAgentEvaluationConfig(aef).BlauSpaceGridding;
					
					IBlauSpaceLattice bsl = BlauSpaceLattice.create(theTemplateDistribution.SampleSpace, steps);
					IBlauSpaceEvaluation meanEval = aeb.MeanEvaluation(bsl);
					IBlauSpaceEvaluation stdEval = aeb.StdEvaluation(bsl);
					IBlauSpaceEvaluation ctEval = aeb.AssignmentCounts(bsl);
					
					BlauSpaceEvaluationGnuplotPresenter pres = BlauSpaceEvaluationGnuplotPresenter.Instance();
					pres.Present(this, meanEval, stdEval);
					
					foreach (IBlauPoint bp in meanEval.AssignedLatticePoints) {
						for (int c=0;c<bp.Space.Dimension;c++) {
							trajWriter.Write(""+bp.getCoordinate(c)+"\t");
						}
						trajWriter.Write(""+meanEval.eval (bp)+"\t");
						trajWriter.Write(""+stdEval.eval (bp)+"\t");
						trajWriter.Write(""+ctEval.eval (bp)+"\t");
						trajWriter.WriteLine("");
					}
					
					trajWriter.Flush ();
					trajWriter.Close ();
				}
				
				foreach (ITrajectoryBundle tb in accumulated_resb.getTrajectoryBundles()) {
					string trajname = TRAJ_FILE_PREFIX+fileName.Substring(DIST_FILE_PREFIX.Length)+".ALL.trajectory."+tb.Name;
					string trajpath = Path.Combine(_trajdir, trajname);
					TextWriter trajWriter = File.CreateText(trajpath);
					
					TrajectoryBundleGnuplotPresenter.Instance().Present(this, tb);
					
					Console.WriteLine("Writing TrajectoryBundle: "+tb.Name);
					
					trajWriter.WriteLine("# TrajectoryBundle: "+tb.Name);
					trajWriter.WriteLine("# time mean std center dev");
					ITrajectory meanTraj = tb.MeanTrajectory;
					ITrajectory stdTraj = tb.StdTrajectory;
					ITrajectory centerTraj = tb.CentralTrajectory;
					ITrajectory devTraj = tb.CentralDevTrajectory;

					foreach (double t in meanTraj.Times) {
						trajWriter.Write(""+meanTraj.eval (t)+"\t");
						trajWriter.Write(""+stdTraj.eval (t)+"\t");
						trajWriter.Write(""+centerTraj.eval (t)+"\t");
						trajWriter.Write(""+devTraj.eval (t)+"\t");
						trajWriter.WriteLine("");
					}
					
					trajWriter.Flush ();
					trajWriter.Close ();
				}
				
				Console.WriteLine("Writing Latex.");
				Latex.Instance().Present(this);	
				CreateReport(this, resultsname);	
				
    		}// distributions
    		*/
		}// run
		
	}// class
}// namespace

