using System;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;

namespace core
{
	public interface IDistribution : IObjectReference
	{
		IBlauSpace SampleSpace {
			get;
		}
		
		IBlauPoint getSample();
		void setTotalSamples(int totalSamples);
		
		int Params {
			get;
		}
		
		bool IsValid();
		
		void DistributionComplete();
		
		string getParamName(int pn);
		
		double getParam(int pn);
		void setParam(int pn, double val);
		
		double getParamMin(int pn);
		void setParamMin(int pn, double val);
		
		double getParamMax(int pn);
		void setParamMax(int pn, double val);
		
		IDistribution clone();
		
		string ToString (int indent);
	}
}

