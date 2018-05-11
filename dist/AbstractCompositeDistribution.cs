using System;
using System.Collections.Generic;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace dist
{
	[Serializable]
	public abstract class AbstractCompositeDistribution : AbstractDistribution
	{
		public AbstractCompositeDistribution(IBlauSpace space, int par) : base(space, par)
		{
		}
		
		protected AbstractCompositeDistribution(AbstractCompositeDistribution orig) : base(orig)
		{
		}
		
		public override void setTotalSamples(int totalSamples) {
			foreach (IDistribution x in this.getComponents()) {
				x.setTotalSamples (totalSamples);
			}
			base.setTotalSamples(totalSamples);
		}

		protected abstract List<IDistribution> getComponents();
		
		protected abstract int AdditionalPerComponentParameters {
			get;
		}
		
		public override bool IsValid() {
			foreach (IDistribution x in this.getComponents()) {
				if ( ! x.IsValid() ) return false;
			}
			return true;
		}
		
		protected abstract double getParam_AdditionalPerComponentParameters(IDistribution x, int pn);
		protected abstract void setParam_AdditionalPerComponentParameters(IDistribution x, int pn, double val);
		
		protected abstract double getParamMin_AdditionalPerComponentParameters(IDistribution x, int pn);
		protected abstract void setParamMin_AdditionalPerComponentParameters(IDistribution x, int pn, double val);
		
		protected abstract double getParamMax_AdditionalPerComponentParameters(IDistribution x, int pn);
		protected abstract void setParamMax_AdditionalPerComponentParameters(IDistribution x, int pn, double val);

		private int getSubdistributionIndex (IDistribution d)
		{
			List<IDistribution> mysubcomp = getComponents ();
			int index = 0;
			foreach (IDistribution sub in mysubcomp) {
				if (d == sub) {
					return index;
				}
				index++;
			}
			return -1;
		}

		public override string getParamName(int pn) {
			int adjusted_pn = 0;
			IDistribution x = GetDistribution(pn, out adjusted_pn);
			if (adjusted_pn >= x.Params) {
				// BK JUNE 2 HASHCODE FIX
				// return "w_"+x.GetHashCode();
				return "w_"+getSubdistributionIndex(x);
			}
			else return x.getParamName(adjusted_pn);
		}
		
		public override double getParam(int pn) {
			int adjusted_pn = 0;
			IDistribution x = GetDistribution(pn, out adjusted_pn);
			if (adjusted_pn >= x.Params) {
				return getParam_AdditionalPerComponentParameters(x, adjusted_pn-x.Params);
			}
			else return x.getParam(adjusted_pn);
		}
		
		public override void setParam(int pn, double val) {
			int adjusted_pn = 0;
			IDistribution x = GetDistribution(pn, out adjusted_pn);
			if (adjusted_pn >= x.Params) {
				setParam_AdditionalPerComponentParameters(x, adjusted_pn-x.Params, val);
			}
			else x.setParam(adjusted_pn, val);
		}
		
		public override double getParamMin(int pn) {
			int adjusted_pn = 0;
			IDistribution x = GetDistribution(pn, out adjusted_pn);
			if (adjusted_pn >= x.Params) {
				return getParamMin_AdditionalPerComponentParameters(x, adjusted_pn-x.Params);
			}
			else return x.getParamMin(adjusted_pn);
		}
		
		public override void setParamMin(int pn, double val) {
			int adjusted_pn = 0;
			IDistribution x = GetDistribution(pn, out adjusted_pn);
			if (adjusted_pn >= x.Params) {
				setParamMin_AdditionalPerComponentParameters(x, adjusted_pn-x.Params, val);
			}
			else x.setParamMin(adjusted_pn, val);
		}
		
		public override double getParamMax(int pn) {
			int adjusted_pn = 0;
			IDistribution x = GetDistribution(pn, out adjusted_pn);
			if (adjusted_pn >= x.Params) {
				return getParamMax_AdditionalPerComponentParameters(x, adjusted_pn-x.Params);
			}
			else return x.getParamMax(adjusted_pn);
		}
		
		public override void setParamMax(int pn, double val) {
			int adjusted_pn = 0;
			IDistribution x = GetDistribution(pn, out adjusted_pn);
			if (adjusted_pn >= x.Params) {
				setParamMax_AdditionalPerComponentParameters(x, adjusted_pn-x.Params, val);
			}
			else x.setParamMax(adjusted_pn, val);
		}
		
		private IDistribution GetDistribution(int pn, out int adjusted_pn) {
			if ((pn < 0) || (pn >= Params)) throw new Exception("AbstractCompositeDistribution GetDistribution index "+pn+" out of range! [0,"+Params+")");
			
			int total = 0;
			foreach (IDistribution x in this.getComponents()) {
				
				int newtotal = total + x.Params;
				newtotal += AdditionalPerComponentParameters;
				
		/*
				Console.WriteLine("  earching for pn: "+pn);
				Console.WriteLine("        subdist x: "+x);
				Console.WriteLine("       has params: "+x.Params);
				Console.WriteLine("    running total: "+newtotal);
		*/
				
				if (newtotal > pn) {
					adjusted_pn = pn - total;
					return x;
				}
				total = newtotal;
			}
			
			//LogStack ();
			throw new Exception ("GetDistribution parameter index out of bounds: "+pn+" not reached by total: "+total);
		}		
		
		/*
		public static void LogStack()
		{
		  var trace = new System.Diagnostics.StackTrace();
		  foreach (var frame in trace.GetFrames())
		  {
		    var method = frame.GetMethod();
		    if (method.Name.Equals("LogStack")) continue;
		    Console.WriteLine(string.Format("{0}::{1}", 
		        method.ReflectedType != null ? method.ReflectedType.Name : string.Empty,
		        method.Name));
		  }
		}
		*/
	}


}

