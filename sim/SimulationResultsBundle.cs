using System;
using System.Collections.Generic;
using core;
using signal;
using metrics;

namespace sim
{
	public class SimulationResultsBundle : ISimulationResultsBundle
	{
		private Dictionary<string, ITrajectoryBundle> _tbundles;
		private Dictionary<string, IAgentEvaluationBundle> _aebundles;
		
		public void add(ISimulationResults res) {
			foreach (ITrajectory t in res.getTrajectories()) {
				ITrajectoryBundle tb = null;
				
				if ( ! _tbundles.ContainsKey(t.Name)) {
					tb = new TrajectoryBundle(t.Name);
					_tbundles.Add(t.Name, tb);
				}
				else {
					tb = _tbundles[t.Name];
				}
				tb.addTrajectory(t);
			}
			
			foreach (IAgentEvaluation ae in res.getAgentEvaluations()) {
				IAgentEvaluationBundle aeb = null;
				if ( ! _aebundles.ContainsKey(ae.Name)) {
					aeb = new AgentEvaluationBundle(ae.Name);
					_aebundles.Add(ae.Name, aeb);
				}
				else {
					aeb = _aebundles[ae.Name];
				}
				aeb.addAgentEvaluation(ae);
			}
		}
		
		public void add(ISimulationResultsBundle resb) {
			
			foreach (ITrajectoryBundle tb in resb.getTrajectoryBundles()) {
				foreach (ITrajectory t in tb.Trajectories) {
					ITrajectoryBundle newtb = null;
					if ( ! _tbundles.ContainsKey(t.Name)) {
						newtb = new TrajectoryBundle(t.Name);
						_tbundles.Add(t.Name, newtb);
					}
					else {
						newtb = _tbundles[t.Name];
					}
					newtb.addTrajectory(t);
				}
			}
			
			foreach (IAgentEvaluationBundle aeb in resb.getAgentEvaluationBundles()) {
				foreach (IAgentEvaluation ae in aeb.Evaluations) {
					IAgentEvaluationBundle newaeb = null;
					if ( ! _aebundles.ContainsKey(ae.Name)) {
						newaeb = new AgentEvaluationBundle(ae.Name);
						_aebundles.Add(ae.Name, newaeb);
					}
					else {
						newaeb = _aebundles[ae.Name];
					}
					aeb.addAgentEvaluation(ae);
				}
			}
		}
		
		
		public IEnumerable<ITrajectoryBundle> getTrajectoryBundles() {
			return _tbundles.Values;
		}
		
		public IEnumerable<IAgentEvaluationBundle> getAgentEvaluationBundles() {
			return _aebundles.Values;
		}
		
		public SimulationResultsBundle ()
		{
			_tbundles = new Dictionary<string, ITrajectoryBundle>();
			_aebundles = new Dictionary<string, IAgentEvaluationBundle>();
		}
		
		public bool Valid {
			get {
				foreach (ITrajectoryBundle trajb in _tbundles.Values) {
					if (!trajb.Valid) return false;
				}
				
				foreach (IAgentEvaluationBundle aeb in _aebundles.Values) {
					if (!aeb.Valid) return false;
				}
				return true;
			}
		}
		
		public override string ToString ()
		{
			string s = "SimulationResultsBundle w/ "+_tbundles.Count+" TrajectoryBundles and "+_aebundles.Count+" AgentEvaluationBundles";
			return s;
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";

			foreach (IAgentEvaluationBundle aeb in _aebundles.Values) {
				s+=aeb.ToStringLong();
				s+="\n";
			}
			
			foreach (ITrajectoryBundle tb in _tbundles.Values) {
				s+=tb.ToStringLong();
				s+="\n";
			}
			
			return s;
		}
	}
}

