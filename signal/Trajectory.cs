using System;
using System.Collections.Generic;
using core;
using logger;
//using C5;

namespace signal
{
	public class Trajectory : ITrajectory
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
		
		private double _temporalGranularityThreshold;
		public double TemporalGranularityThreshold {
			get { return _temporalGranularityThreshold;}
		}
		
		private double _burnin;
		protected double BurninTime {
			get { return _burnin; }
		}
		
		public Trajectory (string name, double timeQuantum, double historicalBias, double burnin)
		{
			_name = name;
			_mint = Double.MaxValue;
			_maxt = Double.MinValue;
			
			_lastTimestampOutput = Double.MinValue;
			_temporalGranularityThreshold = timeQuantum;
			
			_burnin = burnin;
			
			HISTORICAL_BIAS = historicalBias;
			_lastTimestampReported = Double.MinValue;
			_lastHistoricallyAdjustedValue = 0.0;
			_virgin = true;
			
			_values = new C5.TreeDictionary<double, double>();
		}
		
		public double eval(double time) {
			int sz = _values.Keys.Count;
			if (sz==0) return 0.0;
			
			double answer = 0.0;
			if (_values.Contains(time)) {
				answer = _values[time];
			}
			else {
				C5.KeyValuePair<double,double> pre;
				if (_values.TryWeakPredecessor(time, out pre)) {
					C5.KeyValuePair<double,double> post;
					if (_values.TryWeakSuccessor(time, out post)) {
						answer = interpolate(pre.Key, pre.Value,
						                     post.Key, post.Value,
						                     time);
						SingletonLogger.Instance().DebugLog(typeof(Trajectory), "Trajectory "+this.Name+" <>answer:"+answer);
					}
					else {
						pre = _values.FindMax();
						answer = pre.Value;
						SingletonLogger.Instance().DebugLog(typeof(Trajectory), "Trajectory "+this.Name+" >>answer:"+answer);
					}
				}
				else {
					pre = _values.FindMin();
					answer = pre.Value;
					SingletonLogger.Instance().DebugLog(typeof(Trajectory), "Trajectory "+this.Name+" <<answer:"+answer);
				}
			}
			return answer;
		}
		
		private double interpolate(double t1, double v1, double t2, double v2, double time) {
			return v1 + (v2-v1)*(time-t1)/(t2-t1);
		}
		
		private double _lastTimestampOutput;
		private C5.TreeDictionary<double, double> _values;
		
		private double HISTORICAL_BIAS;
		private double _lastTimestampReported;
		private double _lastHistoricallyAdjustedValue;
		private bool _virgin;
		
		protected bool AboveTemporalGranularityThreshold(double time) {
			if (_virgin) {
				return true;
			}
			if ((time - _lastTimestampOutput) >= TemporalGranularityThreshold) {
				return true;
			}
			else {
				return false;
			}
		}
		
		private double ComputeHistoricallyAdjustedValue(double time, double val) {
			double historicallyAdjustedValue;
			if (_virgin) {
				historicallyAdjustedValue = val;
			}
			else {
				double adjustedHistoricalBias = (HISTORICAL_BIAS == 0.0 ? 0.0 : Math.Pow(HISTORICAL_BIAS, time-_lastTimestampReported));
				double presentBias = 1.0 - adjustedHistoricalBias;
				
				historicallyAdjustedValue =
					presentBias*val + adjustedHistoricalBias*_lastHistoricallyAdjustedValue;
			}
			
			_lastTimestampReported = time;
			_lastHistoricallyAdjustedValue = historicallyAdjustedValue;
			_virgin = false;
			
			return historicallyAdjustedValue;
		}
		
		public bool ThresholdTimePassed(double time) {
			return AboveTemporalGranularityThreshold(time);
		}
		
		public void add(double time, double val) {
			
			if ((this.BurninTime > 0.0) && (time < this.BurninTime*3600.0)) return;
			
			SingletonLogger.Instance().DebugLog(typeof(Trajectory), "Trajectory "+this.Name+" attempts add "+time+"->"+val);
			
			double historicallyAdjustedValue = ComputeHistoricallyAdjustedValue(time, val);
			
			// throttle data explosion
			if (!AboveTemporalGranularityThreshold(time)) {
				return;
			}
			
			// maintain function property
			if (_values.Contains(time)) {
				_values.Remove(time);
			}
			_values.Add(time, historicallyAdjustedValue);
			
			SingletonLogger.Instance().DebugLog(typeof(Trajectory), "Trajectory "+this.Name+" adds "+time+"->"+val);
			
			_lastTimestampOutput = time;
			
			// adjust min and max times
			if (time < _mint) _mint = time;
			if (time > _maxt) _maxt = time;
		}
		
		public bool Valid {
			get { return _values.Count!=0; }
		}
		
		public IList<double> Times {
			get { 
				List<double> list = new List<double>();
				list.AddRange(_values.Keys);
				return list; 
			}
		}
		
		public override string ToString ()
		{
			string s = "Trajectory '"+Name+"' timePeriod:("+MinimumTime+" -- "+MaximumTime+") [temporalGranularity:"+_temporalGranularityThreshold+"; historicalBias:"+HISTORICAL_BIAS+"]";
			return s;
		}
		
		private static readonly string TIME_FMT = "{0:0.00}";
		private static readonly string VAL_FMT = "{0:0.0000}";
		
		public string ToStringLong ()
		{
			string s = ""+this+"\n";
			foreach (double t in _values.Keys) {
				double v = _values[t];
				s += ""+String.Format(TIME_FMT, t) +" ==> "+String.Format(VAL_FMT, v) +"\n";
			}
			return s;
		}
	}
}

