using System;
using System.IO;
using core;

namespace presentation
{
	public abstract class AbstractPresenter : IPresenter
	{
		protected AbstractPresenter ()
		{
		}
		
		public abstract string Name {
			get;
		}
		
		private string _target;
		public string Target {
			get { return _target; }
		}
		
		public abstract void Present(IExperiment exp, IPresentable obj);
		public abstract void Present(IExperiment exp, IPresentable mean, IPresentable std);
		public abstract void Present(IExperiment exp);
		
		private System.IO.StreamWriter _file;
		
		protected void BeginPresentation(string dir, string target) {
			_target=target;
			//Console.WriteLine ("Writing to "+PresentationConfig.Directory+_target);
			_file = new System.IO.StreamWriter(Path.Combine(dir,_target));
		}
		
		protected void EndPresentation() {
			_file.Close();
			_file = null;
			_target=null;
		}
		
		protected void AppendToPresentation(string lines) {
			_file.WriteLine(lines.Replace("_","\\_"));
		}
	}
}

