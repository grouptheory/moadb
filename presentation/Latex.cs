using System;
using core;
using System.Collections.Generic;

namespace presentation
{
	public class Latex : AbstractPresenter
	{	
		private static string REPORT_NAME = "report";
		
		private static readonly string NAME = "Latex";
		public override string Name {
			get { return NAME; }
		}
		
		public override void Present(IExperiment exp, IPresentable obj) {
			throw new Exception("Not supported");
		}
		
		public override void Present(IExperiment exp, IPresentable mean, IPresentable std) {
			throw new Exception("Not supported");
		}
		
		private string _experimentName;
		private IExperiment _exp;

		public override void Present(IExperiment exp) {
			_exp = exp;
			_experimentName = exp.Name;
			this.BeginPresentation(PresentationConfig.Directory, REPORT_NAME+"-"+exp.Name+".tex");
			CreateLatex();
			this.EndPresentation();
		}
		
		private static Dictionary<string, string> _images = new Dictionary<string, string>();
		public static void AddImage(string image, string caption) {
			_images.Add(image, caption);
		}
		public static void ClearImages() {
			_images.Clear();
		}
		
		private double SPACING = 0.2;
		
		private void CreateLatex() {
			this.AppendToPresentation("\\documentclass{article}");
			this.AppendToPresentation("\\usepackage{graphics}");
			this.AppendToPresentation("\\setlength\\parindent{0pt}");
			this.AppendToPresentation("\\begin{document}");
			this.AppendToPresentation("  \\title{"+_experimentName+"}");
			this.AppendToPresentation("  \\maketitle");
			
			
			this.AppendToPresentation("\\section{Simulation Setup}");
			
			this.AppendToPresentation("AgentFactoryClassName: "+_exp.theTableConfig.AgentFactoryClassName+"\n");
			this.AppendToPresentation("Number of agents: "+_exp.theTableConfig.NumAgents+"\n");
			this.AppendToPresentation("Populations: "+_exp.theTableConfig.Populations+"\n");
			this.AppendToPresentation("Trials per population: "+_exp.theTableConfig.Trials+"\n");
			this.AppendToPresentation("Duration: "+_exp.theTableConfig.DurationHours+" (hours)\n");
			this.AppendToPresentation("Initial Orderbook: "+_exp.theTableConfig.InitialOrderbook+"\n");
			
			
			this.AppendToPresentation("\\vspace{"+SPACING+"in}");
			
			
			this.AppendToPresentation("Agent BlauSpace Dimension: "+_exp.theBlauSpace.Dimension+"\n");
			this.AppendToPresentation("\\begin{tabular}{|l|l|l|l|}");
			this.AppendToPresentation("\\hline");
			this.AppendToPresentation("Axis & Name & Min & Max\\\\");
			this.AppendToPresentation("\\hline");
			for (int i=0;i<_exp.theBlauSpace.Dimension;i++) {
				this.AppendToPresentation(""+i+" & "+
				                          _exp.theBlauSpace.getAxis(i).Name+" & "+
				                          _exp.theBlauSpace.getAxis(i).MinimumValue+" & "+
				                          _exp.theBlauSpace.getAxis(i).MaximumValue+"\\\\");
			}
			this.AppendToPresentation("\\hline");
			this.AppendToPresentation("\\end{tabular}\n");
			
			this.AppendToPresentation("\\vspace{"+SPACING+"in}");
			
			this.AppendToPresentation("Agents were assigned BlauSpace coordinates via distribution:");
			this.AppendToPresentation("{\\scriptsize");
			this.AppendToPresentation("\\begin{verbatim}");
			this.AppendToPresentation(_exp.theActualDistribution.ToString()+"\n");
			this.AppendToPresentation("\\end{verbatim}");
			this.AppendToPresentation("}");
			
			this.AppendToPresentation("\\vspace{"+SPACING+"in}");
			
			this.AppendToPresentation("Agent/BlauSpace evaluations enabled:\n");
			this.AppendToPresentation("\\begin{tabular}{|l|c|c|}");
			this.AppendToPresentation("\\hline");
			this.AppendToPresentation("Evaluation Name & Metric Name & Bins\\\\");
			this.AppendToPresentation("\\hline");
			foreach (IAgentEvaluationConfig aec in _exp.theAgentEvaluationFactorySetConfig.getAgentEvaluations()) {
				this.AppendToPresentation(""+aec.Name+" & "+
				                          aec.MetricName+" & "+
				                          aec.BlauSpaceGridding+"\\\\");
			}
			this.AppendToPresentation("\\hline");
			this.AppendToPresentation("\\end{tabular}\n");
			
			this.AppendToPresentation("\\vspace{"+SPACING+"in}");
			
			this.AppendToPresentation("Trajectory factories enabled:\n");
			this.AppendToPresentation("\\begin{tabular}{|l|c|c|}");
			this.AppendToPresentation("\\hline");
			this.AppendToPresentation("Trajectory Name & Time Quantum & History Bias\\\\");
			this.AppendToPresentation("\\hline");
			foreach (ITrajectoryFactoryConfig tfc in _exp.theTrajConfig.getTrajectories()) {
				this.AppendToPresentation(""+tfc.Name+" & "+
				                          tfc.MinGranularity+" & "+
				                          tfc.HistoryCoefficient+"\\\\");
			}
			this.AppendToPresentation("\\hline");
			this.AppendToPresentation("\\end{tabular}\n");
			
			int ct=0;
			foreach (KeyValuePair<string,string> kvp in _images) {
				this.AppendToPresentation("  \\begin{figure}[h]");
				this.AppendToPresentation("    \\begin{center}");
				this.AppendToPresentation("      \\resizebox{6in}{!}{\\includegraphics{"+kvp.Key+"}}");
				this.AppendToPresentation("      \\caption{"+kvp.Value+".}");
				this.AppendToPresentation("      \\label{"+kvp.Key+"-"+ct+"}");
				this.AppendToPresentation("    \\end{center}");
			    this.AppendToPresentation("  \\end{figure}");
			    this.AppendToPresentation("  \\clearpage");
			}
			this.AppendToPresentation("\\end{document}");
		}
		
		private static Latex _instance;
		public static Latex Instance() {
			if (_instance==null) {
				_instance = new Latex();
			}
			return _instance;
		}
		
		private Latex ()
		{
		}
	}
}

