using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using core;
using serialization;

namespace dist
{
	[Serializable]
	public class DistributionParameterIteratorSpecification : XmlSerializationIO<DistributionParameterIteratorSpecification>
	{	
		public DistributionParameterIteratorSpecification()
		{
		}
		
		private int _ID;
		public int ID {
			get { return _ID; }
			set { _ID = value; }
		}
		private string _Name;
		public string Name {
			get { return _Name; }
			set { _Name = value; }
		}
		private double _Min;
		public double Min {
			get { return _Min; }
			set { _Min = value; }
		}
		private double _Max;
		public double Max {
			get { return _Max; }
			set { _Max = value; }
		}
		private int _Step;
		public int Step {
			get { return _Step; }
			set { _Step = value; }
		}
	}
	
	[Serializable]
	public class DistributionSpaceIteratorSpecification : XmlSerializationIO<DistributionSpaceIteratorSpecification>, IDistributionSpaceIteratorSpecification
	{
		public static readonly string MODULE = "DistributionSpaceIteratorSpecification";
		
		public DistributionSpaceIteratorSpecification()
		{
			_specs = new List<DistributionParameterIteratorSpecification>();
		}
		
		private static int DEFAULT_STEP = 4;
		
		public DistributionSpaceIteratorSpecification(IDistribution d)
		{
			_specs = new List<DistributionParameterIteratorSpecification>();
			
			for (int i=0; i<d.Params;i++) {
				DistributionParameterIteratorSpecification pis = new DistributionParameterIteratorSpecification();
				pis.ID = i;
				pis.Name = d.getParamName(i);
				pis.Min = d.getParamMin(i);
				pis.Max = d.getParamMax(i);
				pis.Step = DEFAULT_STEP;
				_specs.Add(pis);
			}
		}
		
		private List<DistributionParameterIteratorSpecification> _specs;
		public List<DistributionParameterIteratorSpecification> Specifications {
			get { return _specs; }
			set { _specs = value; }
		}
		
		public int[] ApplyToDistribution (IDistribution d)
		{
			if (d.Params != _specs.Count) {
				throw new Exception ("Incompatible Param count "+d.Params+" != "+_specs.Count+" between DistributionSpaceIteratorSpecification and IDistribution");
			}
			
			int [] steps = new int [d.Params];
			
			/*
			foreach (DistributionParameterIteratorSpecification pis in _specs) {
				int i = pis.ID;
				Console.WriteLine ("" + i + ": " + d.getParamName (i));
			}
			*/

			for (int pnum=0; pnum<d.Params; pnum++) {
				string d_paramname = d.getParamName(pnum);

				DistributionParameterIteratorSpecification pis_found = null;
				foreach (DistributionParameterIteratorSpecification pis in _specs) {
					int i = pis.ID;
					if (d_paramname == pis.Name) {
						pis_found = pis;
						break;
					}
				}
				if (pis_found == null) {
					throw new Exception("Incompatible Parameter# "+pnum+" d:"+d_paramname+" not in DistributionSpaceIteratorSpecification");
				}
				
				d.setParamMin(pnum, pis_found.Min);
				d.setParamMax(pnum, pis_found.Max);
				steps[pnum] = pis_found.Step;
			}

			return steps;
		}
		
		// memoize
		[XmlIgnore()]
		private static Dictionary<string, DistributionSpaceIteratorSpecification> _cache = 
			new Dictionary<string, DistributionSpaceIteratorSpecification>();
			
		public static DistributionSpaceIteratorSpecification Factory(String dir) {
			string FILENAME = dir+"Config."+MODULE+".xml";
			
			if (_cache.ContainsKey(dir)) {
				return _cache[dir];
			}
			
			DistributionSpaceIteratorSpecification instance;
			try {
				instance = (DistributionSpaceIteratorSpecification) DistributionSpaceIteratorSpecification.Load(FILENAME);
				Console.WriteLine(":> DistributionSpaceIteratorSpecification "+FILENAME+" loaded.");
			}
			catch (System.IO.FileNotFoundException) {
				instance= new DistributionSpaceIteratorSpecification(); 
				instance.SaveAs(FILENAME);
				Console.WriteLine(":> DistributionSpaceIteratorSpecification "+FILENAME+" created.");
			}
			
			_cache.Add(dir, instance);
			
			return instance;
		}
	}
}

