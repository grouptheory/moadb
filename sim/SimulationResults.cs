using System;
using System.Collections.Generic;
using core;

namespace sim
{
	public class SimulationResults : ISimulationResults
	{
		public IEnumerable<ITrajectory> getTrajectories() {
			return _tlist;
		}
		
		public IEnumerable<IAgentEvaluation> getAgentEvaluations() {
			return _alist;
		}
		
		public void add(ITrajectory t) {
			_tlist.Add(t);
		}
		
		public void add(IAgentEvaluation a) {
			_alist.Add(a);
		}
		
		private List<ITrajectory> _tlist;
		private List<IAgentEvaluation> _alist;
		
		public SimulationResults()
		{
			_tlist = new List<ITrajectory>();
			_alist = new List<IAgentEvaluation>();
		}
		
		public override string ToString ()
		{
			string s = "*** SimulationResults ***\n";
			s += "Trajectories ("+_tlist.Count+"): ";
			foreach (ITrajectory traj in _tlist) {
				s += (traj.ToString()+", ");
			}
			s += "\n";
			
			s += "AgentEvaluations ("+_alist.Count+"): ";
			foreach (IAgentEvaluation ae in _alist) {
				s += (ae.ToString()+", ");
			}
			s += "\n";
			
			return s;
		}
		
		public bool Valid {
			get {
				foreach (ITrajectory traj in _tlist) {
					if (!traj.Valid) return false;
				}
				
				foreach (IAgentEvaluation ae in _alist) {
					if (!ae.Valid) return false;
				}
				return true;
			}
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			s += "Trajectories (Long):"+_tlist.Count+"\n";
			foreach (ITrajectory traj in _tlist) {
				s += (traj.ToStringLong()+"\n");
			}
			
			s += "AgentEvaluations (Long):"+_alist.Count+"\n";
			foreach (IAgentEvaluation ae in _alist) {
				s += (ae.ToStringLong()+"\n");
			}
			return s;
		}
		
	}
}

