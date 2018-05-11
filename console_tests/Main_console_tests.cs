using System;
using System.IO;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using logger;
using core;
using dist;
using blau;

namespace console_tests
{
	class MainClass
	{

		private static IDistribution create1DGaussian(double mean, double std) {
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IDistribution d = new Distribution_Gaussian(s, mean, std);
			return d;
		}
		
		private static IDistribution create2DGaussian(double mean, double std, double mean2, double std2) {
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IDistribution d = new Distribution_Gaussian(s, mean, std);
			
			int dim2 = 1;
			string [] names2 = new string [1] {"y"};
			double [] mins2 = new double [1] {0.00};
			double [] maxs2 = new double [1] {100.0};
			IBlauSpace s2 = BlauSpace.create(dim2, names2, mins2, maxs2);
			IDistribution d2 = new Distribution_Gaussian(s2, mean2, std2);
			
			int dim3 = 2;
			string [] names3 = new string [2] {"x", "y"};
			double [] mins3 = new double [2] {0.00, 0.00};
			double [] maxs3 = new double [2] {100.0, 100.0};
			IBlauSpace s3 = BlauSpace.create(dim3, names3, mins3, maxs3);
			
			Product d3 = new Product(s3);
			d3.Add(d);
			d3.Add(d2);
			d3.DistributionComplete();
			
			return d3;
		}

		private static IDistribution createPointed (double[] p)
		{
			int dim = p.Length;
			IDistribution [] d = new IDistribution[dim];
			IDistribution prodd = null;
			
			string [] names_all = new string [dim];
			double [] mins_all = new double [dim];
			double [] maxs_all = new double [dim];
			
			IBlauSpace [] s = new IBlauSpace[dim];
			IBlauSpace prods = null;
			
			for (int i=0; i<dim; i++) {
				string [] names = new string [1] {"x"+i};
				double [] mins = new double [1] {0.00};
				double [] maxs = new double [1] {100.0};
				IBlauSpace s1 = BlauSpace.create (1, names, mins, maxs);
				d[i] = new Distribution_Pointed (s1, p [i]);
				
				names_all[i] = names[0];
				mins_all[i] = mins[0];
				maxs_all[i] = maxs[0];
				
				s[i] = BlauSpace.create(i+1, names_all, mins_all, maxs_all);
				if (i==0) {
					prods = s[i];
					prodd = d[i];
				}
				else {
					prods = s[i];
					Product prodd2 = new Product(prods);
					prodd2.Add(prodd);
					prodd2.Add(d[i]);
					prodd2.DistributionComplete();
					prodd = prodd2;
				}
			}
			return prodd;
		}

		public static void Main(string[] args)
		{			
			Console.WriteLine("console_tests");
			
			LoggerInitialization.SetThreshold(typeof(console_tests.MainClass), LogLevel.Debug);
			
			double[] p1 = new double [2] {1.0, 1.0};
			IDistribution d1 = createPointed(p1);
			double[] p2 = new double [2] {70.0, 70.0};
			IDistribution d2 = createPointed(p2);
			
			int dim = 2;
			string [] names = new string [2] {"x0", "x1"};
			double [] mins = new double [2] {0.00, 0.00};
			double [] maxs = new double [2] {100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			Mixture d = new Mixture(s);
			d.Add (d1, 0.75);
			d.Add (d2, 0.25);
			d.DistributionComplete();
			
			SingletonLogger.Instance().DebugLog(typeof(console_tests.MainClass), "original distribution: "+d);
			Console.WriteLine("original distribution: "+d);
			
			DistributionSpace ds = new DistributionSpace(d);
			
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			// N = subdivisions of each of the parameter values
			for (int N=3; N<=3; N++) {
				
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
				
				IDistributionSpaceIterator it = ds.iterator(steps);
				
				int count = 0;
				int validCt = 0;
				foreach (IDistribution diter in it) {
					if ( diter.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(console_tests.MainClass), "iterator distribution: "+diter);
						Console.WriteLine("valid distribution: "+diter);
					}
					else {
						Console.WriteLine("invalid distribution: "+diter);
					}
					count++; 
				}
				
				Console.WriteLine("# of valid distributions: "+validCt);
				Console.WriteLine("# of total distributions: "+count);
				
				// Assert.AreEqual((N+1)*(N+1)*(N+1)*(N+1)*(N+1)*(N+1), count);
				// SingletonLogger.Instance().InfoLog(typeof(console_tests.MainClass), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}

		public static void Main2DMixture (string[] args)
		{			
			Console.WriteLine("console_tests");
			
			LoggerInitialization.SetThreshold(typeof(console_tests.MainClass), LogLevel.Debug);

			double mean = 70.0;
			double std = 1.0;
			IDistribution d1 = create2DGaussian(mean, std, mean, std);
			double mean2 = 20.0;
			double std2 = 1.0;
			IDistribution d2 = create2DGaussian(mean2, std2, mean2, std2);
			
			int dim = 2;
			string [] names = new string [2] {"x", "y"};
			double [] mins = new double [2] {0.00, 0.00};
			double [] maxs = new double [2] {100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			Mixture d = new Mixture(s);
			d.Add (d1, 0.75);
			d.Add (d2, 0.25);
			d.DistributionComplete();
			
			SingletonLogger.Instance().DebugLog(typeof(console_tests.MainClass), "original distribution: "+d);
			Console.WriteLine("original distribution: "+d);
			
			DistributionSpace ds = new DistributionSpace(d);
			
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			// N = subdivisions of each of the parameter values
			for (int N=3; N<=3; N++) {
				
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
				
				IDistributionSpaceIterator it = ds.iterator(steps);
				
				int count = 0;
				int validCt = 0;
				foreach (IDistribution diter in it) {
					if ( diter.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(console_tests.MainClass), "iterator distribution: "+diter);
						Console.WriteLine("valid distribution: "+diter);
					}
					else {
						Console.WriteLine("invalid distribution: "+diter);
					}
					count++; 
				}
				
				Console.WriteLine("# of valid distributions: "+validCt);
				Console.WriteLine("# of total distributions: "+count);
				
				// Assert.AreEqual((N+1)*(N+1)*(N+1)*(N+1)*(N+1)*(N+1), count);
				// SingletonLogger.Instance().InfoLog(typeof(console_tests.MainClass), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}


		public static void Main1DMixture (string[] args)
		{			
			Console.WriteLine("console_tests");

			LoggerInitialization.SetThreshold(typeof(console_tests.MainClass), LogLevel.Debug);

			double mean = 70.0;
			double std = 1.0;
			IDistribution d1 = create1DGaussian(mean, std);
			double mean2 = 20.0;
			double std2 = 1.0;
			IDistribution d2 = create1DGaussian(mean2, std2);
			
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);

			Mixture d = new Mixture(s);
			d.Add (d1, 0.75);
			d.Add (d2, 0.25);
			d.DistributionComplete();

			SingletonLogger.Instance().DebugLog(typeof(console_tests.MainClass), "original distribution: "+d);
			Console.WriteLine("original distribution: "+d);

			DistributionSpace ds = new DistributionSpace(d);

			int [] steps = new int[ds.ParamSpace.Dimension];

			// N = subdivisions of each of the parameter values
			for (int N=3; N<=6; N++) {

				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
				
				IDistributionSpaceIterator it = ds.iterator(steps);
				
				int count = 0;
				int validCt = 0;
				foreach (IDistribution diter in it) {
					if ( diter.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(console_tests.MainClass), "iterator distribution: "+diter);
						Console.WriteLine("valid distribution: "+diter);
					}
					else {
						Console.WriteLine("invalid distribution: "+diter);
					}
					count++; 
				}
				
				Console.WriteLine("# of valid distributions: "+validCt);
				Console.WriteLine("# of total distributions: "+count);

				Assert.AreEqual((N+1)*(N+1)*(N+1)*(N+1)*(N+1)*(N+1), count);
				SingletonLogger.Instance().InfoLog(typeof(console_tests.MainClass), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}
	}
}
