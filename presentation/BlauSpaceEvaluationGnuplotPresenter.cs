using System;
using core;

namespace presentation
{
	public class BlauSpaceEvaluationGnuplotPresenter : AbstractPresenter
	{
		private static readonly string NAME = "BlauSpaceEvaluationGnuplotPresenter";
		public override string Name {
			get { return NAME; }
		}
		
		private string _experimentName;
		private IExperiment _exp;
		public override void Present(IExperiment exp, IPresentable obj) {
			throw new Exception("Not supported");
		}

		public override void Present(IExperiment exp) {
			throw new Exception("Not supported");
		}
		
		public override void Present(IExperiment exp, IPresentable mean, IPresentable std) {
			_experimentName = exp.Name;
			_exp = exp;
			
			for (int c = 0; c<exp.theBlauSpace.Dimension; c++) {
				Present((IBlauSpaceEvaluation)mean, (IBlauSpaceEvaluation)std, c);
			}
		}
		
		private void Present(IBlauSpaceEvaluation bse, IBlauSpaceEvaluation std, int c) {
			this.BeginPresentation(_datadir, bse.Name+""+c+".bse");
			
			string header = "# BlauSpaceEvaluation "+bse.Name+"";
			this.AppendToPresentation(header);
			header = "# "+bse.Lattice.BlauSpace+" mean std";
			this.AppendToPresentation(header);
			
			foreach (IBlauPoint p in bse.AssignedLatticePoints) {
				string val = ""+bse.eval(p);
				string bar = ""+std.eval(p);
				string str = ""+p.getCoordinate(c)+"\t"+val+"\t"+bar;
				this.AppendToPresentation(str);
			}
			
			this.EndPresentation();
	
			this.BeginPresentation(_gpdir, bse.Name+""+c+".bse.gp");
			CreateGnuplotScript(bse, std, c);
			this.EndPresentation();
		}
		
		private void CreateGnuplotScript(IBlauSpaceEvaluation bse, IBlauSpaceEvaluation std, int c) {
			this.AppendToPresentation("set terminal postscript eps enhanced");
			this.AppendToPresentation("set output \""+bse.Name+""+c+".eps\"");
			string axisName = _exp.theBlauSpace.getAxis(c).Name;
			Latex.AddImage(""+bse.Name+""+c+".eps", "Influence of "+axisName+" (BlauSpace coord "+c+") on "+bse.Name+"");
			
			this.AppendToPresentation("set title \""+_experimentName+" "+bse.Name+" Agent Evaluation\"");
			this.AppendToPresentation("set ylabel \""+bse.Name+"\"");
			this.AppendToPresentation("set xlabel \""+axisName+" (BlauSpace coord "+c+")\""); 
			this.AppendToPresentation("set autoscale y"); 
			string s = "plot \"../"+_exp.BSE_DIR_STRING+"/"+ bse.Name+""+c+".bse\" using 1:2:3 with yerrorbars t \""+bse.Name+"\" lc 1";
			this.AppendToPresentation(s);
		}
		
		private static readonly double STEPS = 100.0;

		private void Present(ITrajectory traj, int i, int n) {
			double step = (traj.MaximumTime - traj.MinimumTime)/STEPS;
			for (double x=traj.MinimumTime; x<=traj.MaximumTime; x+=step) { /// NULL TRAJECTORY FIX
				double y = traj.eval(x);
				string str = ""+x+"\t"+y+"\n";
				this.AppendToPresentation(str);

				if (step==0.0) break; /// NULL TRAJECTORY FIX
			}
		}

		/*
		private static BlauSpaceEvaluationGnuplotPresenter _instance;
		public static BlauSpaceEvaluationGnuplotPresenter Instance() {
			if (_instance==null) {
				_instance = new BlauSpaceEvaluationGnuplotPresenter();
			}
			return _instance;
		}
		*/
		
		private string _gpdir;
		private string _datadir;
		
		public BlauSpaceEvaluationGnuplotPresenter (string datadir, string gpdir)
		{
			_datadir = datadir;
			_gpdir = gpdir;
		}
	}
}

