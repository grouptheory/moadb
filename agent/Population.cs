using System;
using System.Collections.Generic;
using core;
using logger;

namespace agent
{
	/// <summary>
    /// Population class
    /// </summary>
	public class Population : IPopulation
	{
		// agents (not necessarily of homogenous type)
		private List<IAgent> _agents;
		
		// indexed by name
		private Dictionary<string, IAgent> _name2agent;
		
		// population size
		public int Size {
			get { return _agents.Count; }
		}
		
		// constructor
		public Population ()
		{
			_agents = new List<IAgent>();
			_name2agent = new Dictionary<string, IAgent>();
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(Population), "Population constructed");
		}
		
		// get agent by index
		public IAgent getAgent(int i) {
			return _agents[i];
		}
		
		// get agent by name
		public IAgent getAgent(string name) {
			if (!_name2agent.ContainsKey(name)) {
				throw new Exception("Population could find no Agent named "+name);
			}
			return _name2agent[name];
		}
		
		// add an agent
		public void addAgent(IAgent ag) {
			if (_agents.Contains(ag)) {
				throw new Exception("Population is attempting to add a duplicate IAgent");
			}
			_agents.Add(ag);
			_name2agent.Add(ag.GetName(), ag);
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(Population), "Population added: "+ag);
		}
		
		// remove an agent
		public void removeAgent(IAgent ag) {
			if ( ! _agents.Contains(ag)) {
				throw new Exception("Population is attempting to remove a nonexistent IAgent");
			}
			_agents.Remove(ag);
			_name2agent.Remove(ag.GetName());
			if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(Population), "Population removed: "+ag);
		}
		
		// IEnumerable<IAgent> Members -- this makes the population enumerable
		public IEnumerator<IAgent> GetEnumerator()
		{
			foreach (IAgent ag in _agents)
			{
				yield return ag;
			}
		}

		//IEnumerable Members -- this makes the population enumerable
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _agents.GetEnumerator();
		}
		
		// clone the population
		public IPopulation clone() {
			IPopulation p2 = new Population();
			foreach (IAgent ag in _agents)
			{
				p2.addAgent(ag.clone());
			}
			return p2;
		}
		
		// delegate signals from the Simulation to appropriate agents
		public void recvSimulationNotification(ISimulationParameters sim, ISimulationEvent se) {
			
			if (se.OrderbookEvent == null) {
				// start and end events go to all agents
				if (se is ISimulationStart) {
					foreach (IAgent ag in _agents)
					{
						ag.recvSimulationNotification(sim, se);
					}
				}
				else if (se is ISimulationEnd) {
					foreach (IAgent ag in _agents)
					{
						ag.recvSimulationNotification(sim, se);
					}
				}
			}
			else {
				// fill events go to the order owner only
				if (se.OrderbookEvent is IOrderbookEvent_FillOrder) {
					IOrderbookEvent_FillOrder fillEvent = (IOrderbookEvent_FillOrder)se.OrderbookEvent;
					IOrderOwner owner = fillEvent.getOrder().getOwner();
					if (_agents.Contains((IAgent)owner)) {
						owner.recvOrderNotification(sim.Orderbook, fillEvent); 
					}
					else {
						throw new Exception("Population is attempting to forward signal to an unrecognized IAgent");
					}
				}
				// Add and Cancel events are ignored by the Population class
			}
		}

		public bool partition (IBlauSpace s, IBlauSpaceAxis axis, double myValue, int myId)
		{
			// filter pop based on myValue on axis
			IList<IAgent> filteredSet = new List<IAgent> ();
			foreach (IAgent ag in this) {

				if (ag is IAgent_NonParticipant) continue;

				int axisIndex = s.getAxisIndex (axis.Name);
				double agentCoord = ag.Coordinates.getCoordinate (axisIndex);

				// POTENTIAL PROBLEM WITH FLOATING POINT
				if (agentCoord == myValue) {
					filteredSet.Add (ag);
				}
			}
			// get size of filtered set
			int fcount = filteredSet.Count;
			int c = 0;
			foreach (IAgent fa in filteredSet) {
				if (fa.ID == myId) {
					return true;
				}
				c++;
				if (c > myValue*fcount) {
					return false;
				}
			}
			throw new Exception("Wierd inconsistency in population partition");
			// return false;
		}

		// diagnostic to string
		public override string ToString() {
			string str = "";
			foreach (IAgent ag in this) {
				str += ""+ag+"\n";
			}
			return str;
		}
	}
}

