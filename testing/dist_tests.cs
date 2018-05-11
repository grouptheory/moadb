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

namespace testing
{
	[TestFixture()]
	public class dist_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(dist_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(Mixture), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(BlauSpace), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
		
		private IDistribution create1DGaussian(double mean, double std) {
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IDistribution d = new Distribution_Gaussian(s, mean, std);
			return d;
		}
		
		[Test()]
		public void Distribution_Gaussian1DTest()
		{
			Console.WriteLine("Distribution_Gaussian1DTest");
			
			double mean = 70.0;
			double std = 1.0;
			IDistribution d = create1DGaussian(mean, std);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			for (int i=0;i<1000;i++) {
				IBlauPoint p = d.getSample();
				SingletonLogger.Instance().DebugLog(typeof(dist_tests), "=> "+p);
				
				double diff = Math.Abs(p.getCoordinate(0)-mean);
				Assert.AreEqual((diff > 5.0*std), false);
			}
		}
		
		[Test()]
		public void Distribution_IteratorSpecTest()
		{
			Console.WriteLine("Distribution_IteratorSpecTest");
			
			double mean = 70.0;
			double std = 1.0;
			IDistribution d = create1DGaussian(mean, std);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			DistributionSpaceIteratorSpecification spec = new DistributionSpaceIteratorSpecification(d);
			spec.SaveAs("spec.xml");
		}
		
		private IDistribution createGaussianMixture(double mean, double std, double mean2, double std2) {
			
			IDistribution d1 = create1DGaussian(mean, std);
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
			
			return d;
		}
		
		[Test()]
		public void Distribution_GaussianMixtureTest()
		{
			Console.WriteLine("Distribution_GaussianMixtureTest");
			
			double mean = 70.0;
			double std = 1.0;
			double mean2 = 20.0;
			double std2 = 1.0;
			IDistribution d = createGaussianMixture(mean, std, mean2, std2);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			for (int i=0;i<1000;i++) {
				IBlauPoint p = d.getSample();
				SingletonLogger.Instance().DebugLog(typeof(dist_tests), "=> "+p);
				
				double diff = Math.Abs(p.getCoordinate(0)-mean);
				double diff2 = Math.Abs(p.getCoordinate(0)-mean2);
				Assert.AreEqual((diff > 5.0*std) && (diff2 > 5.0*std2) , false);
			}
		}
		
		[Test()]
		public void Distribution_GaussianMixtureSerializationTest()
		{
			Console.WriteLine("Distribution_GaussianMixtureSerializationTest");
			
			double mean = 70.0;
			double std = 1.0;
			double mean2 = 20.0;
			double std2 = 1.0;
			IDistribution d = createGaussianMixture(mean, std, mean2, std2);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			SoapFormatter formatter = new SoapFormatter();
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "SERIALIZING");
			
			FileStream fs = new FileStream("gaussianmixture.xml", FileMode.Create);
			formatter.Serialize(fs, d);
    		fs.Close();
					
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "DESERIALIZING");
			
			fs = new FileStream("gaussianmixture.xml", FileMode.Open);
			IDistribution  d2 = (IDistribution)formatter.Deserialize(fs);
			fs.Close();
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "DONE");
			
			Assert.AreEqual(d.SampleSpace==d2.SampleSpace, true);
			
			Assert.AreEqual(d is Mixture, true);
			Assert.AreEqual(d2 is Mixture, true);
			
			Mixture g1 = (Mixture)d;
			Mixture g2 = (Mixture)d2;
			
			Assert.AreEqual(g1.Params.CompareTo(g2.Params), 0);
			g1.IsValid();
			g2.IsValid();
			
			Assert.AreEqual(g1.IsValid().CompareTo(g2.IsValid()), 0);
			
			for (int i=0;i<g1.Params;i++) {
				Assert.AreEqual(g1.getParam(i).CompareTo(g2.getParam(i)), 0);
				Assert.AreEqual(g1.getParamMin(i).CompareTo(g2.getParamMin(i)), 0);
				Assert.AreEqual(g1.getParamMax(i).CompareTo(g2.getParamMax(i)), 0);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "All distributions coincide as expected");
		}
		
		[Test()]
		public void Distribution_Interval1DTest()
		{
			Console.WriteLine("Distribution_Interval1DTest");

			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			double min = 10.0;
			double max = 30.0;
			IDistribution d = new Distribution_Interval(s, min, max);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			for (int i=0;i<1000;i++) {
				IBlauPoint p = d.getSample();
				SingletonLogger.Instance().DebugLog(typeof(dist_tests), "=> "+p);
				
				for (int x=0;x<dim;x++) {
					double diff = p.getCoordinate(x)-min;
					Assert.AreEqual(diff<0.0, false);
					
					diff = max-p.getCoordinate(x);
					Assert.AreEqual(diff<0.0, false);
				}
			}
		}
		
		[Test()]
		public void Distribution_GaussianSerializationTest()
		{
			Console.WriteLine("Distribution_Gaussian1DSerializationTest");

			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			double mean = 70.0;
			double std = 1.0;
			IDistribution d = new Distribution_Gaussian(s, mean, std);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			SoapFormatter formatter = new SoapFormatter();
			
			FileStream fs = new FileStream("gaussian.xml", FileMode.Create);
			formatter.Serialize(fs, d);
    		fs.Close();
					
			fs = new FileStream("gaussian.xml", FileMode.Open);
			IDistribution  d2 = (IDistribution)formatter.Deserialize(fs);
			fs.Close();
			
			Assert.AreEqual(d.SampleSpace==d2.SampleSpace, true);
			Assert.AreEqual(d is Distribution_Gaussian, true);
			Assert.AreEqual(d2 is Distribution_Gaussian, true);
			
			Distribution_Gaussian g1 = (Distribution_Gaussian)d;
			Distribution_Gaussian g2 = (Distribution_Gaussian)d2;
			
			Assert.AreEqual(g1.Mean.CompareTo(g2.Mean), 0);
			Assert.AreEqual(g1.Std.CompareTo(g2.Std), 0);
			Assert.AreEqual(g1.Params, g2.Params);
			Assert.AreEqual(g1.SampleSpace, g2.SampleSpace);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "All distributions coincide as expected");
		}
		
		[Test()]
		public void Distribution_SequentialSerializationTest()
		{
			Console.WriteLine("Distribution_SequentialSerializationTest");

			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			IDistribution d = new Distribution_Sequential(s, 0.1);
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			SoapFormatter formatter = new SoapFormatter();
			
			FileStream fs = new FileStream("seq.xml", FileMode.Create);
			formatter.Serialize(fs, d);
    		fs.Close();
					
			fs = new FileStream("seq.xml", FileMode.Open);
			IDistribution  d2 = (IDistribution)formatter.Deserialize(fs);
			fs.Close();
			
			Assert.AreEqual(d.SampleSpace==d2.SampleSpace, true);
			Assert.AreEqual(d is Distribution_Sequential, true);
			Assert.AreEqual(d2 is Distribution_Sequential, true);
			
			Distribution_Sequential g1 = (Distribution_Sequential)d;
			Distribution_Sequential g2 = (Distribution_Sequential)d2;
			
			Assert.AreEqual(g1.Step, g2.Step);
			Assert.AreEqual(g1.Params, g2.Params);
			Assert.AreEqual(g1.SampleSpace, g2.SampleSpace);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "All distributions coincide as expected");
		}
		
		[Test()]
		public void Distribution_IntervalSerializationTest()
		{
			Console.WriteLine("Distribution_IntervalSerializationTest");

			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			double min = 10.0;
			double max = 30.0;
			IDistribution d = new Distribution_Interval(s, min, max);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			SoapFormatter formatter = new SoapFormatter();
			
			FileStream fs = new FileStream("interval.xml", FileMode.Create);
			formatter.Serialize(fs, d);
    		fs.Close();
					
			fs = new FileStream("interval.xml", FileMode.Open);
			IDistribution  d2 = (IDistribution)formatter.Deserialize(fs);
			fs.Close();
			
			Assert.AreEqual(d.SampleSpace==d2.SampleSpace, true);
			Assert.AreEqual(d is Distribution_Interval, true);
			Assert.AreEqual(d2 is Distribution_Interval, true);
			
			Distribution_Interval g1 = (Distribution_Interval)d;
			Distribution_Interval g2 = (Distribution_Interval)d2;
			
			Assert.AreEqual(g1.Max, g2.Max);
			Assert.AreEqual(g1.Min, g2.Min);
			Assert.AreEqual(g1.Params, g2.Params);
			Assert.AreEqual(g1.SampleSpace, g2.SampleSpace);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "All distributions coincide as expected");
		}
		
		private IDistribution create2DGaussian(double mean, double std, double mean2, double std2) {
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

		private IDistribution createPointed (double[] p)
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

		[Test()]
		public void Distribution_Gaussian2DTest() {
			Console.WriteLine("Distribution_Gaussian2DTest");
			
			double mean = 70.0;
			double std = 1.0;
			double mean2 = 20.0;
			double std2 = 1.0;
			
			IDistribution g2d = create2DGaussian(mean, std, mean2, std2);
			
			for (int i=0;i<100;i++) {
				IBlauPoint p = g2d.getSample();
				SingletonLogger.Instance().DebugLog(typeof(Main_dist), "=> "+p);
				
				Assert.AreEqual((Math.Abs(p.getCoordinate(0) - mean) > 10.0*std), false);
				Assert.AreEqual((Math.Abs(p.getCoordinate(1) - mean2) > 10.0*std2), false);
			}
		}
		
		[Test()]
		public void Distribution_Gaussian2DSerializationTest()
		{
			Console.WriteLine("Distribution_Gaussian2DSerializationTest");

			double mean = 70.0;
			double std = 1.0;
			double mean2 = 20.0;
			double std2 = 1.0;
			
			IDistribution d = create2DGaussian(mean, std, mean2, std2);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "distribution: "+d);
			
			SoapFormatter formatter = new SoapFormatter();
			
			FileStream fs = new FileStream("gaussian2d.xml", FileMode.Create);
			formatter.Serialize(fs, d);
    		fs.Close();
					
			fs = new FileStream("gaussian2d.xml", FileMode.Open);
			IDistribution  d2 = (IDistribution)formatter.Deserialize(fs);
			fs.Close();
			
			Assert.AreEqual(d.SampleSpace==d2.SampleSpace, true);
			Assert.AreEqual(d is Product, true);
			Assert.AreEqual(d2 is Product, true);
			
			Product g1 = (Product)d;
			Product g2 = (Product)d2;
			
			Assert.AreEqual(g1.Params.CompareTo(g2.Params), 0);
			Assert.AreEqual(g1.IsValid().CompareTo(g2.IsValid()), 0);
			for (int i=0;i<g1.Params;i++) {
				Assert.AreEqual(g1.getParam(i).CompareTo(g2.getParam(i)), 0);
				Assert.AreEqual(g1.getParamMin(i).CompareTo(g2.getParamMin(i)), 0);
				Assert.AreEqual(g1.getParamMax(i).CompareTo(g2.getParamMax(i)), 0);
			}
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "All distributions coincide as expected");
		}
		
		[Test()]
		public void DistributionSpaceIterator_SingleGaussianTest()
		{
			Console.WriteLine("DistributionSpaceIterator_SingleGaussianTest");
			
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			double mean = 70.0;
			double std = 1.0;
			IDistribution d = new Distribution_Gaussian(s, mean, std);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "original distribution: "+d);
			DistributionSpace ds = new DistributionSpace(d);
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			for (int N=3; N<=5; N++) {
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
			
				IDistributionSpaceIterator it = ds.iterator(steps);
			
				int count = 0;
				int validCt = 0;
				foreach (IDistribution d2 in it) {
					if ( d2.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(dist_tests), "iterator distribution: "+d2);
					}
					count++;
				}
				Assert.AreEqual((N+1)*(N+1), count);
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}
		
		
		[Test()]
		public void DistributionSpaceIteratorReverse_SingleGaussianTest()
		{
			Console.WriteLine("DistributionSpaceIteratorReverse_SingleGaussianTest");
			
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			double mean = 70.0;
			double std = 1.0;
			IDistribution d = new Distribution_Gaussian(s, mean, std);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "original distribution: "+d);
			DistributionSpace ds = new DistributionSpace(d);
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			for (int N=3; N<=5; N++) {
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
			
				IDistributionSpaceIterator it = ds.iterator(steps);
			
				int count = 0;
				int validCt = 0;
				foreach (IDistribution d2 in it) {
					if ( d2.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(dist_tests), "iterator distribution: "+d2);
					}
					count++;
				}
				Assert.AreEqual((N+1)*(N+1), count);
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}
		
		[Test()]
		public void DistributionSpaceIterator_SingleIntervalTest()
		{
			Console.WriteLine("DistributionSpaceIterator_SingleIntervalTest");
			
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			double min = 20.0;
			double max = 80.0;
			IDistribution d = new Distribution_Interval(s, min, max);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "original distribution: "+d);
			DistributionSpace ds = new DistributionSpace(d);
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			for (int N=3; N<=5; N++) {
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
			
				IDistributionSpaceIterator it = ds.iterator(steps);
			
				int count = 0;
				int validCt = 0;
				foreach (IDistribution d2 in it) {
					if ( d2.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(dist_tests), "iterator distribution: "+d2);
					}
					
					count++;
				}
				Assert.AreEqual((N+1)*(N+1), count);
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}
		
		[Test()]
		public void DistributionSpaceIterator_SingleSequentialTest()
		{
			Console.WriteLine("DistributionSpaceIterator_SingleSequentialTest");
			
			int dim = 1;
			string [] names = new string [1] {"x"};
			double [] mins = new double [1] {0.00};
			double [] maxs = new double [1] {100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			double step = 1.0;
			IDistribution d = new Distribution_Sequential(s, step);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "original distribution: "+d);
			DistributionSpace ds = new DistributionSpace(d);
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			for (int N=3; N<=5; N++) {
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
			
				IDistributionSpaceIterator it = ds.iterator(steps);
			
				int count = 0;
				int validCt = 0;
				foreach (IDistribution d2 in it) {
					if ( d2.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(dist_tests), "iterator distribution: "+d2);
					}
					count++;
				}
				Assert.AreEqual(1, count);
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}
		
		[Test()]
		public void DistributionSpaceIterator_Gaussian2D() {
			Console.WriteLine("DistributionSpaceIterator_Gaussian2D");
			
			double mean = 70.0;
			double std = 1.0;
			double mean2 = 20.0;
			double std2 = 1.0;
			
			IDistribution d = create2DGaussian(mean, std, mean2, std2);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "original distribution: "+d);
			DistributionSpace ds = new DistributionSpace(d);
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			for (int N=3; N<=5; N++) {
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
			
				IDistributionSpaceIterator it = ds.iterator(steps);
			
				int count = 0;
				int validCt = 0;
				foreach (IDistribution d2 in it) {
					if ( d2.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(dist_tests), "iterator distribution: "+d2);
					}
					count++;
				}
				Assert.AreEqual((N+1)*(N+1)*(N+1)*(N+1), count);
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  valid distributions: "+validCt);
			}
		}

		[Test()]
		public void DistributionSpaceIterator_Pointed2D() {
			Console.WriteLine("DistributionSpaceIterator_Pointed2D");
			
			double [] p1 = new double [] {70.0, 20.0, 4.1};
			
			IDistribution d = createPointed(p1);
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "DistributionSpaceIterator_Pointed2D distribution: "+d);
			DistributionSpace ds = new DistributionSpace(d);
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			for (int N=3; N<=5; N++) {
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
				
				IDistributionSpaceIterator it = ds.iterator(steps);
				
				int count = 0;
				int validCt = 0;
				foreach (IDistribution d2 in it) {
					if ( d2.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(dist_tests), "iterator distribution: "+d2);
					}
					count++;
				}
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  distributions: "+count);
				Assert.AreEqual(Math.Pow ((double)(N+1), (double)(ds.ParamSpace.Dimension)), count);
				
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  valid distributions: "+validCt);
				Assert.AreEqual(Math.Pow ((double)(N+1), (double)(ds.ParamSpace.Dimension)), validCt);
			}
		}

		[Test()]
		public void DistributionSpaceIterator_GaussianMixtureTest()
		{
			Console.WriteLine("DistributionSpaceIterator_GaussianMixtureTest");
			
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
			
			
			SingletonLogger.Instance().DebugLog(typeof(dist_tests), "original distribution: "+d);
			DistributionSpace ds = new DistributionSpace(d);
			int [] steps = new int[ds.ParamSpace.Dimension];
			
			for (int N=3; N<=5; N++) {
				for (int i=0;i<ds.ParamSpace.Dimension; i++) steps[i]=N;
			
				IDistributionSpaceIterator it = ds.iterator(steps);
			
				int count = 0;
				int validCt = 0;
				foreach (IDistribution diter in it) {
					if ( diter.IsValid() ) {
						validCt++;
						SingletonLogger.Instance().DebugLog(typeof(dist_tests), "iterator distribution: "+diter);
					}
					count++;
				}
				Assert.AreEqual((N+1)*(N+1)*(N+1)*(N+1)*(N+1)*(N+1), count);
				SingletonLogger.Instance().InfoLog(typeof(dist_tests), "N="+N+"  valid distributions: "+validCt+" / total: "+count);
			}
		}
	}
}

