using System;
using core;
using signal;

namespace presentation
{
	public class TrajectoryBundleGnuplotPresenter : AbstractPresenter
	{
		private static readonly string NAME = "BlauSpaceEvaluationGnuplotPresenter";
		public override string Name {
			get { return NAME; }
		}
		
		private string _experimentName;
		private IExperiment _exp;
		
		public override void Present(IExperiment exp, IPresentable obj) {
			_experimentName = exp.Name;
			_exp = exp;
			Present((ITrajectoryBundle)obj);
		}
		
		public override void Present(IExperiment exp, IPresentable mean, IPresentable std) {
			throw new Exception("Not supported");
		}
		
		public override void Present(IExperiment exp) {
			throw new Exception("Not supported");
		}
		
		private void Present(ITrajectoryBundle tb) {
			int i=1;
			foreach (ITrajectory traj in tb.Trajectories) {
				this.BeginPresentation(_datadir, traj.Name+"-"+i+".tra");
				Present (traj, i, tb.Trajectories.Count);
				this.EndPresentation();
				i++;
			}
			
			ITrajectory meanTraj = tb.MeanTrajectory;
			ITrajectory stdTraj = tb.StdTrajectory;
			ITrajectory centerTraj = tb.CentralTrajectory;
			ITrajectory centerDev = tb.CentralDevTrajectory;
			
			this.BeginPresentation(_datadir, meanTraj.Name+".tra");
			PresentMean (meanTraj, stdTraj);
			this.EndPresentation();
			
			this.BeginPresentation(_datadir, centerTraj.Name+TrajectoryBundleCollapser_CentralDTW.SUFFIX+".tra");
			PresentCenter (centerTraj, centerDev);
			this.EndPresentation();
			
			this.BeginPresentation(_gpdir, tb.Name+".traj.gp");
			CreateGnuplotScript(tb, meanTraj, centerTraj);
			this.EndPresentation();
		}
		
		private void ComputeYrange(ITrajectoryBundle tb, out double min, out double max) {
			min = Double.MaxValue;
			max = Double.MinValue;
			foreach (ITrajectory traj in tb.Trajectories) {
				double step = (traj.MaximumTime - traj.MinimumTime)/STEPS;
				for (double x=traj.MinimumTime; x<=traj.MaximumTime; x+=step) { /// NULL TRAJECTORY FIX
					double y = traj.eval(x);
					if (y<min) min=y;
					if (y>max) max=y;
					
					if (step==0.0) break; /// NULL TRAJECTORY FIX
				}
			}
		}

		private void ComputeXrange(ITrajectoryBundle tb, out double min, out double max) {
			min = Double.MaxValue;
			max = Double.MinValue;
			foreach (ITrajectory traj in tb.Trajectories) {
				if (traj.MinimumTime < min) min=traj.MinimumTime;
				if (traj.MaximumTime > max) max=traj.MaximumTime;
			}
		}

		private void CreateGnuplotScript(ITrajectoryBundle tb, ITrajectory meanTraj, ITrajectory centerTraj) {
			this.AppendToPresentation("set terminal postscript eps enhanced");
			this.AppendToPresentation("set output \""+tb.Name+".eps\"");
			Latex.AddImage(""+tb.Name+".eps", ""+tb.Name+" trajectory in "+_experimentName);
			
			double min, max;
			ComputeYrange(tb, out min, out max);
			double low = min==Double.MaxValue ? -1.0 : min-(max-min);
			double high = max==Double.MinValue ? 1.0 : max+(max-min);

			double mint, maxt;
			ComputeXrange(tb, out mint, out maxt);
			double lowt = mint==Double.MaxValue ? 0.0 : mint;
			double hight = maxt==Double.MinValue ? 1.0 : maxt;
			
			this.AppendToPresentation("set xrange [ "+lowt+" : "+hight+" ]");
			this.AppendToPresentation("set yrange [ "+low+" : "+high+" ]");

			this.AppendToPresentation("set title \""+_experimentName+" "+tb.Name+" Trajectory\"");
			this.AppendToPresentation("set ylabel \""+tb.Name+"\"");
			this.AppendToPresentation("set xlabel \"Time (seconds)\""); 
			string s = "plot ";
			
			int i=1;
			foreach (ITrajectory traj in tb.Trajectories) {
				s += "\"../"+_exp.TRAJ_DIR_STRING+"/"+traj.Name+"-"+i+".tra\" using 1:2 with lines notitle lt 2 lc rgb \"black\"";
				s+=", ";
				i++;
			}
			s += "\"../"+_exp.TRAJ_DIR_STRING+"/"+meanTraj.Name+".tra\" using 1:2:3 with yerrorbars t \"Mean\" lc rgb \"blue\",";
			s += "\"../"+_exp.TRAJ_DIR_STRING+"/"+centerTraj.Name+TrajectoryBundleCollapser_CentralDTW.SUFFIX+".tra\" using 1:2:3 with yerrorbars t \"Center\" lc rgb \"red\"";
			this.AppendToPresentation(s);
		}
		
		private static readonly double STEPS = 100.0;
		
		private void Present(ITrajectory traj, int i, int n) {
			string header = "# Number "+i+" of "+n+"";
			this.AppendToPresentation(header);
			Present (traj);
		}
		
		private void Present(ITrajectory traj) {
			string header = "# Trajectory "+traj.Name+"";
			this.AppendToPresentation(header);
			header = "# time value";
			this.AppendToPresentation(header);

			int count=0;
			double step = (traj.MaximumTime - traj.MinimumTime)/STEPS;
			for (double x=traj.MinimumTime; x<=traj.MaximumTime; x+=step) { /// NULL TRAJECTORY FIX
				double y = traj.eval(x);
				string str = ""+x+"\t"+y+"";
				this.AppendToPresentation(str);

				if (step==0.0) break; /// NULL TRAJECTORY FIX

				count++;
				if (count >= STEPS) {
					break;
				}
			}
		}
		
		private void PresentMean(ITrajectory mean, ITrajectory std) {
			string header = "# Trajectory "+mean.Name+"";
			this.AppendToPresentation(header);
			header = "# time mean std";
			this.AppendToPresentation(header);
			
			int count=0;
			double step = (mean.MaximumTime - mean.MinimumTime)/STEPS;
			for (double x=mean.MinimumTime; x<=mean.MaximumTime; x+=step) { /// NULL TRAJECTORY FIX
				double y = mean.eval(x);
				double bar = std.eval(x);
				string str = ""+x+"\t"+y+"\t"+bar;
				this.AppendToPresentation(str);

				if (step==0.0) break; /// NULL TRAJECTORY FIX

				count++;
				if (count >= STEPS) {
					break;
				}
			}
		}
		
		private void PresentCenter(ITrajectory center, ITrajectory dev) {
			string header = "# Trajectory "+center.Name+TrajectoryBundleCollapser_CentralDTW.SUFFIX;
			this.AppendToPresentation(header);
			header = "# time center dev";
			this.AppendToPresentation(header);
			
			int count=0;
			double step = (center.MaximumTime - center.MinimumTime)/STEPS;
			for (double x=center.MinimumTime; x<=center.MaximumTime; x+=step) { /// NULL TRAJECTORY FIX
				double ctr = center.eval(x);
				double delta = dev.eval(x);
				string str = ""+x+"\t"+ctr+"\t"+delta;
				this.AppendToPresentation(str);
				
				if (step==0.0) break; /// NULL TRAJECTORY FIX

				count++;
				if (count >= STEPS) {
					break;
				}
			}
		}

		/*
		private static TrajectoryBundleGnuplotPresenter _instance;
		public static TrajectoryBundleGnuplotPresenter Instance() {
			if (_instance==null) {
				_instance = new TrajectoryBundleGnuplotPresenter();
			}
			return _instance;
		}
		*/

		private string _gpdir;
		private string _datadir;

		public TrajectoryBundleGnuplotPresenter (string datadir, string gpdir)
		{
			_datadir = datadir;
			_gpdir = gpdir;
		}
	}
}

