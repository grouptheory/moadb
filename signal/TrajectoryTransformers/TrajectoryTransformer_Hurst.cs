using System;
using System.Collections.Generic;
using core;
using logger;

namespace signal
{
	public class TrajectoryTransformer_Hurst : ITrajectoryTransformer
	{
		private static readonly double LOG2 = Math.Log (2.0);
		private static readonly int MIN_REG_POINTS = 3;
		private static readonly int INITIAL_T2COUNT = 32; 
		private static readonly double EXPSCALE = 1.1; 
		
		public ITrajectory eval(ITrajectory orig) {
			double finish = orig.MaximumTime;
			// double start = orig.MinimumTime;
			
			ITrajectory Hurst = new Trajectory(orig.Name+"-Hurst("+_timeWindow+","+_timeStep+")", 0.0, 0.0, 0.0);
			
			/*
			for (double t = start; t <= finish; t+= _timeStep) {
				double exponent = ComputeHurstExponent(t, orig);
				if ( ! Double.IsNaN (exponent)) {
					Hurst.add(t, exponent);
				}
			}
			*/
			
			double exponent = ComputeHurstExponent(finish, orig);
			SingletonLogger.Instance().InfoLog(typeof(TrajectoryTransformer_Hurst), "Hurst exponent: "+exponent);
			
			return Hurst;
		}
		
		private double ComputeHurstExponent(double timeOfInterest, ITrajectory orig) {
			double finish = timeOfInterest;
			double start = Math.Max(finish - _timeWindow, orig.MinimumTime);
			
			int n = 0;
			for (double t = start; t <= finish; t+= _timeStep) { n++; }
			
			if (n<1)  {
				SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "n<1");
				return Double.NaN;
			}
			
			int numpoints = 0;
			int t2count = INITIAL_T2COUNT;
			while (t2count < n) {
				numpoints++;
				t2count = (int)((double)t2count * EXPSCALE);
			}
			if (numpoints < MIN_REG_POINTS) {
				SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "numpoints = "+numpoints);
				return Double.NaN;
			}
			
			
			
			ITrajectory X = new Trajectory("X", 0.0, 0.0, 0.0);
			double sigmaX = 0.0;
			n = 0;
			for (double t = start; t <= finish; t+= _timeStep) {
				double Xt = orig.eval(t);
				X.add(t, Xt);
				sigmaX += Xt;
				n++;
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "X:"+X.ToStringLong());
			
			if (n==0) {
				throw new Exception("Divide by n=0");
			}
			double m = sigmaX / (double)n;
			
			ITrajectory Y = new Trajectory("Y", 0.0, 0.0, 0.0);
			foreach (double t in X.Times) {
				Y.add (t, X.eval(t) - m);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "Y:"+Y.ToStringLong());
			
			ITrajectory Z = new Trajectory("Z", 0.0, 0.0, 0.0);
			double sigmaY = 0.0;
			foreach (double t in Y.Times) {
				sigmaY += Y.eval(t);
				Z.add (t, sigmaY);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "Z:"+Z.ToStringLong());
			
			ITrajectory R = new Trajectory("R", 0.0, 0.0, 0.0);
			double Zmax = Double.MinValue;
			double Zmin = Double.MaxValue;
			foreach (double t in Z.Times) {
				double Zt = Z.eval(t);
				Zmax = Math.Max (Zmax, Zt);
				Zmin = Math.Min (Zmin, Zt);
				R.add (t, Zmax-Zmin);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "R:"+R.ToStringLong());
			
			ITrajectory U = new Trajectory("U", 0.0, 0.0, 0.0);
			sigmaX = 0.0;
			int tcounter = 0;
			foreach (double t in X.Times) {
				sigmaX += X.eval(t);
				tcounter++;
				U.add(t, sigmaX / (double)tcounter);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "U:"+U.ToStringLong());
			
			ITrajectory S = new Trajectory("S", 0.0, 0.0, 0.0);
			double sigmaDevXU2 = 0.0;
			tcounter = 0;
			foreach (double t in X.Times) {
				double DevXU = (X.eval(t) - U.eval (t));
				sigmaDevXU2 += (DevXU * DevXU);
				tcounter++;
				S.add(t, Math.Sqrt(sigmaDevXU2 / (double)tcounter));
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "S:"+S.ToStringLong());
			
			double EPSI = 0.00001;
			
			ITrajectory RoverS = new Trajectory("R/S", 0.0, 0.0, 0.0);
			foreach (double t in R.Times) {
				double St = S.eval (t);
				double Rt = R.eval (t);
				if (St<EPSI) {
					if (Rt<EPSI) {
						RoverS.add(t,  0.0);
					}
					else {
						// throw new Exception("Divide Rt!=0 by St=0");
					}
				}
				else {
					RoverS.add(t, Rt / St);
					SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "At t="+t+", R="+Rt+" and S="+St+" hence R/S="+Rt/St);
				}
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "xxx R/S:"+RoverS.ToStringLong());
			
			double[] xvalues = new double[n];
			double[] yvalues = new double[n];
			
			for (int point=0; point<n; point++) {
				double t = start + (point * _timeStep);
				xvalues[point] = (double)point+1.0;
				yvalues[point] = RoverS.eval(t);
				SingletonLogger.Instance().InfoLog(typeof(TrajectoryTransformer_Hurst), "t="+xvalues[point]+" RS="+yvalues[point]);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "EXPONENTIALIZED");
			
			double[] xvaluesAve = new double[n];
			double[] yvaluesAve = new double[n];
			int cutoff = INITIAL_T2COUNT;
			double sumX=0.0;
			double sumY=0.0;
			int averaged=0;
			int bins=0;
			
			for (int point=cutoff; point<n; point++) {
				if (xvalues[point]==0.0) {
					SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "Skipping t="+xvalues[point]+" RS="+yvalues[point]);
					continue;
				}
				
				if (yvalues[point]==0.0) {
					SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "Skipping t="+xvalues[point]+" RS="+yvalues[point]);
					continue;
				}
				
				sumX+=xvalues[point];
				sumY+=yvalues[point];
				averaged++;
				
				if (point == cutoff) {
					xvaluesAve[bins] = sumX / (double)averaged;
					yvaluesAve[bins] = sumY / (double)averaged;
					
					SingletonLogger.Instance().InfoLog(typeof(TrajectoryTransformer_Hurst), "_t="+xvaluesAve[bins]+" _RS="+yvaluesAve[bins]);

					cutoff = (int)((double)cutoff * EXPSCALE);
					sumX=0.0;
					sumY=0.0;
					averaged=0;
					bins++;
				}
				
			}
			
			SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "LOGGED");
			
			double[] xvaluesLog = new double[bins];
			double[] yvaluesLog = new double[bins];
			t2count = INITIAL_T2COUNT;
			
			for (int point=0; point<bins; point++) {
				if (xvaluesAve[point]==0.0) {
					SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "Skipping t="+xvaluesAve[point]+" RS="+yvaluesAve[point]);
					continue;
				}
				xvaluesLog[point] = Math.Log (xvaluesAve[point]) / LOG2;
				
				if (yvaluesAve[point]==0.0) {
					SingletonLogger.Instance().DebugLog(typeof(TrajectoryTransformer_Hurst), "Skipping t="+xvaluesAve[point]+" RS="+yvaluesAve[point]);
					continue;
				}
				yvaluesLog[point] = Math.Log (yvaluesAve[point]) / LOG2;
				
				SingletonLogger.Instance().InfoLog(typeof(TrajectoryTransformer_Hurst), "logt="+xvaluesLog[point]+" logRS="+yvaluesLog[point]);
			}
			
			double slope, yintercept;
			DoLinearRegression(xvaluesLog, yvaluesLog, out slope, out yintercept);
			
			return slope;
		}
		
		private void DoLinearRegression(double[] xvalues, double[] yvalues, out double slope, out double yintercept) {
			double xAvg = 0;
			double yAvg = 0;
			for (int x = 0; x < xvalues.Length; x++)
			{
				xAvg += xvalues[x];
				yAvg += yvalues[x];
			}
			xAvg = xAvg / (double)xvalues.Length;
			yAvg = yAvg / (double)xvalues.Length;
			
			double v1 = 0;
        	double v2 = 0;
			for (int x = 0; x < xvalues.Length; x++)
			{
				v1 += (xvalues[x] - xAvg) * (yvalues[x] - yAvg);
				v2 += Math.Pow(xvalues[x] - xAvg, 2);
			}
			
			if (v2==0.0) {
				throw new Exception("Divide by v2=0");
			}
			
			slope = v1 / v2;
			yintercept = yAvg - slope * xAvg;
		}
		
		private double _timeWindow;
		private double _timeStep;
		
		public TrajectoryTransformer_Hurst (double timeWindow, double timeStep)
		{
			_timeWindow = timeWindow;
			_timeStep = timeStep;
		}
	}
}

