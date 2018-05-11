using System;
using core;
using logger;

namespace metrics
{
	public class NamedMetricAgentEvaluationFactory : AbstractAgentEvaluationFactory
	{
		public override void SimulationStartNotification() {
		}
		
		public override void SimulationEndNotification() {
			
			SingletonLogger.Instance().DebugLog(typeof(NamedMetricAgentEvaluationFactory), "NamedMetricAgentEvaluationFactory got SimulationEndNotification");
			
			foreach (IAgent ag in Population) {
				
				if (ag is IAgent_NonParticipant) {
					continue;
				}
				
				double val = ag.GetMetricValue(Name);
				MyAgentEvaluation.set(ag, val);
			}
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			// no op
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			// no op
		}
		public override void NewOrderNotification(IOrder newOrder) {
			// no op
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder) {
			// no op
		}
		
		public NamedMetricAgentEvaluationFactory (string metricName) : base(metricName)
		{
		}
		
		public override string ToString ()
		{
			string s = "NamedMetricAgentEvaluationFactory ("+base.ToString ()+")";
			return s;
		}
	}
}

