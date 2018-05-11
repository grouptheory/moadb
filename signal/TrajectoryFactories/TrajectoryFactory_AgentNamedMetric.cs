using System;
using core;
using logger;

namespace signal
{
	public class TrajectoryFactory_AgentNamedMetric : AbstractAgentTrajectoryFactory
	{
		private static string NAME = "AgentNamedMetric";
		private string _name;
		public override string Name {
			get {return _name+"-"+_agent.GetName()+"-"+_metricName;}
		}
		
		public override void SimulationEndNotification() {
			MyTrajectory.add(TimeNow, _agent.GetMetricValue(_metricName) );
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			if (filledOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.GetMetricValue(_metricName) );
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			if (partialOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.GetMetricValue(_metricName) );
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
			if (newOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.GetMetricValue(_metricName) );
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
			if (cancelledOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.GetMetricValue(_metricName) );
		}
		
		private string _metricName;
		
		public TrajectoryFactory_AgentNamedMetric(IAgent agent, string metricName, double timeQuantum, double historicalBias) : base(agent, timeQuantum, historicalBias) {
			_name = NAME;
			_metricName = metricName;
			reset();
		}
	}
}

