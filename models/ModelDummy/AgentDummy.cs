using System;
using System.Collections.Generic;
using core;
using agent;
using logger;
using des;

namespace models
{
	public class AgentDummy : AbstractAgent
	{
		protected override string BASENAME {
			get { return "Agent_Dummy"; }
		}
		
		public AgentDummy(IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0) { }
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume)         { }
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) { }
		
		private readonly static string NetWorth_METRICNAME = "NetWorth";
		public static readonly double MEANWORTH = 1.0;
		
		public override void SimulationStartNotification(IPopulation pop) { 
			SingletonLogger.Instance().DebugLog(typeof(AgentDummy), "*** "+this+" got ISimulationStart @ "+Scheduler.GetTime());
			SetMetricValue(NetWorth_METRICNAME, SingletonRandomGenerator.Instance.NextGaussian(MEANWORTH, 0.2));
		}
		public override void SimulationEndNotification() { 
			SingletonLogger.Instance().DebugLog(typeof(AgentDummy), "*** "+this+" got ISimulationEnd @ "+Scheduler.GetTime());
		}
		
		protected override double GetTimeToNextActionPrompt() { 
			return 1.0; 
		}
		
		protected override bool DecideToAct() { 
			SingletonLogger.Instance().DebugLog(typeof(AgentDummy), "*** "+this+" got DecideToAct() @ "+Scheduler.GetTime());
			return false; 
		}
		
		protected override bool DecideToCancelOpenOrder(IOrder openOrder) { return false; }
		protected override bool DecideToMakeOrder()                       { return false; }
		protected override bool DecideToSubmitBid()                       { return false; }
		protected override double GetBidPrice()                           { return 1.0; }
		protected override int GetBidVolume()                             { return 1; }
		protected override double GetAskPrice()                           { return 1.0; }
		protected override int GetAskVolume()                             { return 1; }
	}
}

