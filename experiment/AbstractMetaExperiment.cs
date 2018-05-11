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
	public abstract class AbstractMetaExperiment : IMetaExperiment
	{
		protected string _outdir;
		
		public static readonly string METAEXPERIMENT_PREFIX = "metaexperiment";

		public static string DIST_DIR = "distributions";
		public string DIST_DIR_STRING {
			get { return DIST_DIR; }
		}
		protected static readonly string DIST_FILE_PREFIX = "dist";
		protected string _distdir;
		
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
		
		public static readonly string TEMPLATE_DIST_FILE = "TemplateDistribution.xml";
		private void LoadTemplateDistribution() {
			string distribfile = Path.Combine(_outdir, TEMPLATE_DIST_FILE);
			SoapFormatter formatter = new SoapFormatter();
			FileStream fs = new FileStream(distribfile, FileMode.Open);
			IDistribution  d = (IDistribution)formatter.Deserialize(fs);
			fs.Close();
			SingletonLogger.Instance().DebugLog(typeof(AbstractExperiment), "Template distribution: "+d);
			theTemplateDistribution = d;
		}

		private IDistribution _templateDistribution;
		public IDistribution theTemplateDistribution {
			get { return _templateDistribution; }
			set { _templateDistribution = value; }
		}
		
		private IDistributionSpaceIteratorSpecification _iterspec;
		public IDistributionSpaceIteratorSpecification theDistributionSpaceIteratorSpecification {
			get { return _iterspec; }
			set { _iterspec = value; }
		}
		
		public IBlauSpace theBlauSpace {
			get { return theTemplateDistribution.SampleSpace; }
		}

		private static readonly string DIST_SPACE_ITER_SPEC_FILE = "DistributionIteratorSpec.xml";	
		private void LoadDistributionSpaceIteratorSpecification() {
			string specfile = Path.Combine(_outdir, DIST_SPACE_ITER_SPEC_FILE);
			theDistributionSpaceIteratorSpecification = DistributionSpaceIteratorSpecification.Load(specfile);
		}

		public AbstractMetaExperiment (string rootdir, string expname, string fullname, bool create)
		{
			Name = expname;
			
			_outdir = Path.Combine(rootdir, fullname);
			if ( ! Directory.Exists(_outdir)) {
				if (create) Directory.CreateDirectory(_outdir);
				else throw new Exception("Directory "+_outdir+" does not exist!");
			}

			_distdir = Path.Combine(_outdir, DIST_DIR);
			if ( ! Directory.Exists(_distdir)) {
				if (create) Directory.CreateDirectory(_distdir);
				else throw new Exception("Directory "+_distdir+" does not exist!");
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
				
				string distribfile = Path.Combine(rootdir, TEMPLATE_DIST_FILE);
				if ( ! File.Exists(distribfile)) {
					throw new Exception("File "+distribfile+" does not exist!");
				}
				File.Copy(distribfile, Path.Combine(_outdir, TEMPLATE_DIST_FILE));

				string iterfile = Path.Combine(ApplicationConfig.EXECDIR, DIST_SPACE_ITER_SPEC_FILE);
				if ( ! File.Exists(iterfile)) {
					throw new Exception("File "+iterfile+" does not exist!");
				}
				File.Copy(iterfile, Path.Combine(_outdir, DIST_SPACE_ITER_SPEC_FILE));
			}
			
			TableConfig = TableGenerationConfig.Factory(_outdir);
			TrajConfig = TrajectoryFactorySetConfig.Factory(_outdir);
			EvalConfig = AgentEvaluationFactorySetConfig.Factory(_outdir);

			LoadTemplateDistribution();
			LoadDistributionSpaceIteratorSpecification();
		}

		public abstract void run();
	}
}

