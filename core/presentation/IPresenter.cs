using System;

namespace core
{
	public interface IPresenter
	{
		string Name {
			get;
		}
		
		string Target {
			get;
		}
		
		void Present(IExperiment exp, IPresentable obj);
		void Present(IExperiment exp, IPresentable mean, IPresentable std);
		void Present(IExperiment exp);
	}
}

