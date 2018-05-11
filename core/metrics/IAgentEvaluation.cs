using System;

namespace core
{
	public interface IAgentEvaluation : INamedObject
	{	
		bool Valid {
			get;
		}
		
		IAgentEvaluationFactory Creator {
			get;
		}
		
		void set(IAgent ag, double val);
		double eval(IAgent ag);
		
		void AddToBlauSpaceMultiEvaluation(IBlauSpaceMultiEvaluation bse);
		
		string ToStringLong ();
	}
}

