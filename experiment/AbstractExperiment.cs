using System;
using System.IO;
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


namespace experiment
{
	public abstract class AbstractExperiment : IExperiment
	{
		protected string _root;
		protected string _outdir;

		public static readonly string EXPERIMENT_PREFIX = "experiment";

		public static readonly string POP_DIR = "populations";
		public string POP_DIR_STRING {
			get { return POP_DIR; }
		}
		protected static readonly string POP_FILE_PREFIX = "pop";
		protected string _popdir;
		
		public static readonly string TRAJ_DIR = "trajectories";
		public string TRAJ_DIR_STRING {
			get { return TRAJ_DIR; }
		}
		protected static readonly string TRAJ_FILE_PREFIX = "traj";
		protected string _trajdir;
		
		public static readonly string BSE_DIR = "evaluations";
		public string BSE_DIR_STRING {
			get { return BSE_DIR; }
		}
		protected static readonly string BSE_FILE_PREFIX = "bse";
		protected string _bsedir;

		public static string RESULTS_DIR = "results";
		public string RESULTS_DIR_STRING {
			get { return RESULTS_DIR; }
		}

		private string _name;
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		
		public string OutputDirectory {
			get { return _outdir; }
			set { _outdir = value; }
		}
		
		private TableGenerationConfig TableConfig;
		public ITableGenerationConfig theTableConfig {
			get { return TableConfig; }
		}
		
		private TrajectoryFactorySetConfig TrajConfig;
		public ITrajectoryFactorySetConfig theTrajConfig {
			get { return TrajConfig; }
		}
		
		private IAgentEvaluationFactorySetConfig EvalConfig;
		public IAgentEvaluationFactorySetConfig theAgentEvaluationFactorySetConfig {
			get { return EvalConfig; }
		}
		
		protected AbstractExperiment(string rootdir, string expname, string fullname, bool create) {
			Name = expname;
			_resb = null;
			_simb = null;

			_root = rootdir;

			_outdir = Path.Combine(rootdir, fullname);
			if ( ! Directory.Exists(_outdir)) {
				if (create) Directory.CreateDirectory(_outdir);
				else throw new Exception("Directory "+_outdir+" does not exist!");
			}
			_popdir = Path.Combine(_outdir, POP_DIR);
			if ( ! Directory.Exists(_popdir)) {
				if (create) Directory.CreateDirectory(_popdir);
				else throw new Exception("Directory "+_popdir+" does not exist!");
			}
			_trajdir = Path.Combine(_outdir, TRAJ_DIR);
			if ( ! Directory.Exists(_trajdir)) {
				if (create) Directory.CreateDirectory(_trajdir);
				else throw new Exception("Directory "+_trajdir+" does not exist!");
			}
			_bsedir = Path.Combine(_outdir, BSE_DIR);
			if ( ! Directory.Exists(_bsedir)) {
				if (create) Directory.CreateDirectory(_bsedir);
				else throw new Exception("Directory "+_bsedir+" does not exist!");
			}
			
			if (create) {
				if ( ! File.Exists(TableGenerationConfig.FileName(rootdir))) {
					throw new Exception("File "+TableGenerationConfig.FileName(rootdir)+" does not exist!");
				}
				File.Copy(TableGenerationConfig.FileName(rootdir), TableGenerationConfig.FileName(_outdir));
				
				if ( ! File.Exists(TrajectoryFactorySetConfig.FileName(rootdir))) {
					throw new Exception("File "+TrajectoryFactorySetConfig.FileName(rootdir)+" does not exist!");
				}
				File.Copy(TrajectoryFactorySetConfig.FileName(rootdir), TrajectoryFactorySetConfig.FileName(_outdir));
				
				if ( ! File.Exists(AgentEvaluationFactorySetConfig.FileName(rootdir))) {
					throw new Exception("File "+AgentEvaluationFactorySetConfig.FileName(rootdir)+" does not exist!");
				}
				File.Copy(AgentEvaluationFactorySetConfig.FileName(rootdir), AgentEvaluationFactorySetConfig.FileName(_outdir));
				
				string distribfile = Path.Combine(rootdir, ACTUAL_DIST_FILE);
				if ( ! File.Exists(distribfile)) {
					throw new Exception("File "+distribfile+" does not exist!");
				}
				File.Copy(distribfile, Path.Combine(_outdir, ACTUAL_DIST_FILE));
			}
			
			TableConfig = TableGenerationConfig.Factory(_outdir);
			TrajConfig = TrajectoryFactorySetConfig.Factory(_outdir);
			EvalConfig = AgentEvaluationFactorySetConfig.Factory(_outdir);

			LoadActualDistribution();
		}

		public static readonly string ACTUAL_DIST_FILE = "ActualDistribution.xml";
		private void LoadActualDistribution() {
			string distribfile = Path.Combine(_outdir, ACTUAL_DIST_FILE);
			SoapFormatter formatter = new SoapFormatter();
			FileStream fs = new FileStream(distribfile, FileMode.Open);
			IDistribution  d = (IDistribution)formatter.Deserialize(fs);
			fs.Close();
			SingletonLogger.Instance().DebugLog(typeof(AbstractExperiment), "Actual distribution: "+d);
			theActualDistribution = d;
		}
		
		private IDistribution _actualDistribution;
		public IDistribution theActualDistribution {
			get { return _actualDistribution; }
			set { _actualDistribution = value; }
		}
		
		public IBlauSpace theBlauSpace {
			get { return theActualDistribution.SampleSpace; }
		}

		
		protected static IAgentFactory CreateAgentFactory(IDistribution d, string agentFactoryName) {
			Assembly assembly = typeof(AgentDummy).Assembly; 
			string assemblyName = assembly.GetName().Name;
			Type type = assembly.GetType(assemblyName+"."+agentFactoryName); 
			object[] args = new object[1];
			args[0] = d;
			var agentFactory = (IAgentFactory)Activator.CreateInstance(type, args);
			IAgentFactory afact = (IAgentFactory)agentFactory;
			return afact;
		}
		
		protected static IPopulation CreatePopulation(IAgentFactory afact, int numAgents, string initialOrderbook) {
			
			// SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "pre.");
			IPopulation pop = PopulationFactory.Instance().create(afact, numAgents);
			// SingletonLogger.Instance().InfoLog(typeof(SingleDistributionExperiment), "post.");
			
			AgentOrderbookLoader loader = Helpers.MakeAgentOrderbookLoader(initialOrderbook);
			pop.addAgent(loader);
			
			return pop;
		}
		
		protected static IAgentEvaluationFactory CreateAgentEvaluationFactory(AgentEvaluationConfig aefc) {
			string evaluationFactoryName = aefc.Name;
			Assembly assembly = typeof(NamedMetricAgentEvaluationFactory).Assembly; 
			string assemblyName = assembly.GetName().Name;
			Type type = assembly.GetType(assemblyName+"."+evaluationFactoryName); 
			object[] args = new object[1];
			args[0] = aefc.MetricName;
			var aef = (IAgentEvaluationFactory)Activator.CreateInstance(type, args);
			return (IAgentEvaluationFactory)aef;
		}
		
		protected static IPassiveTrajectoryFactory CreatePassiveTrajectoryFactory(TrajectoryFactoryConfig tfc) {
			string trajectoryFactoryName = tfc.Name;
			Assembly assembly = typeof(TrajectoryFactory_Price).Assembly; 
			string assemblyName = assembly.GetName().Name;
			Type type = assembly.GetType(assemblyName+"."+trajectoryFactoryName); 
			object[] args = new object[2];
			args[0] = tfc.MinGranularity;
			args[1] = tfc.HistoryCoefficient;
			var tf = (IPassiveTrajectoryFactory)Activator.CreateInstance(type, args);
			return (IPassiveTrajectoryFactory)tf;
		}
		
		protected ISimulationResultsBundle _resb;
		protected ISimulationBundle _simb;
		
		public ISimulationBundle getSimulationBundle() {
			return _simb;
		}
		
		public ISimulationResultsBundle getSimulationResultsBundle() {
			return _resb;
		}
		
		public abstract void run();
	}
}

