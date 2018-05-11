using System;
using core;
using logger;

namespace signal
{
	public class TrajectoryFactory_AgentBids : AbstractAgentTrajectoryFactory
	{
		private static string NAME = "AgentBids";
		private string _name;
		public override string Name {
			get {return _name+"-"+_agent.GetName();}
		}
		
		public override void SimulationEndNotification() {
			MyTrajectory.add(TimeNow, _agent.NumBids );
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume) {
			if (filledOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.NumBids );
		}
		
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) {
			if (partialOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.NumBids );
		}
		
		public override void NewOrderNotification(IOrder newOrder) {
			if (newOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.NumBids );
		}
		
		public override void CancelOrderNotification(IOrder cancelledOrder){
			if (cancelledOrder.getOwner() == _agent)
				MyTrajectory.add(TimeNow, _agent.NumBids );
		}
		
		public TrajectoryFactory_AgentBids(IAgent agent, double timeQuantum, double historicalBias) : base(agent, timeQuantum, historicalBias) {
			_name = NAME;
			reset();
		}
	}
}

