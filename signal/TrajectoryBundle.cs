using System;
using System.Collections.Generic;
using core;

namespace signal
{
	public class TrajectoryBundle : ITrajectoryBundle
	{
		private string _name;
		public string Name {
			get {return _name;}
		}
		
		private double _mint;
		public double MinimumTime {
			get {return _mint;}
		}
		
		private double _maxt;
		public double MaximumTime {
			get {return _maxt;}
		}
		
		public ITrajectory MeanTrajectory {
			get {return getMeanTrajectory();}
		}
		
		public ITrajectory StdTrajectory {
			get {return getStdTrajectory();}
		}
		
		public ITrajectory CentralTrajectory {
			get {return getCentralTrajectory();}
		}
		
		public ITrajectory CentralDevTrajectory {
			get {return getCentralDevTrajectory();}
		}
		
		private LinkedList<ITrajectory> _trajectories;
		
		public bool Valid {
			get {
				foreach (ITrajectory traj in _trajectories) {
					if (!traj.Valid) return false;
				}
				return true;
			}
		}
		
		public bool addTrajectory(ITrajectory trajectory) {
			if ( ! trajectory.Name.Equals(this.Name)) {
				throw new Exception("Nonhomogenous TrajectoryBundle");
			}
			
			_trajectories.AddLast(trajectory);
			if (trajectory.MinimumTime < _mint) _mint = trajectory.MinimumTime;
			if (trajectory.MaximumTime > _maxt) _maxt = trajectory.MaximumTime;
			
			if (trajectory.TemporalGranularityThreshold > _temporalGranularityThreshold) {
				_temporalGranularityThreshold = trajectory.TemporalGranularityThreshold;
			}
			
			return true;
		}
		
		public TrajectoryBundle (string name)
		{
			_name = name;
			_mint = Double.MaxValue;
			_maxt = Double.MinValue;
			_trajectories = new LinkedList<ITrajectory>();
			_temporalGranularityThreshold = 0.0;
		}
		
		public bool Empty {
			get { return Times.Count==0; }
		}
		
		private double _temporalGranularityThreshold;
		public double TemporalGranularityThreshold {
			get { return _temporalGranularityThreshold;}
		}
		
		public SortedList<double,double> Times {
			get {
				SortedList<double,double> alltimes = new SortedList<double,double>();
				
				foreach (ITrajectory traj in _trajectories) {
					foreach (double t in traj.Times) {
						if ( ! alltimes.ContainsKey(t)) {
							alltimes.Add(t,t);
						}
					}
				}
				return alltimes;
			}
		}
		
		private ITrajectory getMeanTrajectory() {
			return TrajectoryBundleCollapser_Mean.Instance().eval(this);
		}
		
		private ITrajectory getStdTrajectory() {
			return TrajectoryBundleCollapser_Std.Instance().eval(this);
		}
		
		private ITrajectory getCentralTrajectory() {
			return TrajectoryBundleCollapser_CentralDTW.Instance().eval(this);
		}
		
		private ITrajectory getCentralDevTrajectory() {
			return TrajectoryBundleCollapser_MaxDTWDeviation.Instance().eval(this);
		}
		
		public List<ITrajectory> Trajectories {
			get {
				List<ITrajectory> list = new List<ITrajectory>();
				list.AddRange(_trajectories);
				return list;
			}
		}
		
		public override string ToString ()
		{
			string s = "TrajectoryBundle '"+Name+"' ("+MinimumTime+" -- "+MaximumTime+") w/ "+_trajectories.Count+" Trajectories";
			return s;
		}
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			foreach (ITrajectory traj in _trajectories) {
				s += ""+traj.ToStringLong()+"\n";
			}
			return s;
		}
	}
}

