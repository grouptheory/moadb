using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using logger;
using core;
using blau;
using metrics;

namespace testing
{
	[TestFixture()]
	public class blau_tests
	{
		[TestFixtureSetUp()]
		public void setup() {
			LoggerInitialization.SetThreshold(typeof(blau_tests), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(BlauSpaceRegistry), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(BlauSpaceLatticeRegistry), LogLevel.Debug);
			LoggerInitialization.SetThreshold(typeof(BlauSpaceLattice), LogLevel.Debug);
		}
		
		[TestFixtureTearDown()]
		public void teardown() {
		}
	
		void setCoordinateBadly1() {
		}
		
		[Test()]
		public void BlauPointTest()
		{
			Console.WriteLine("BlauPointTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			IBlauPoint bp = new BlauPoint(s);
			bp.setCoordinate(0, 10.0);
			bp.setCoordinate(1, 20.0);
			bp.setCoordinate(2, 30.0);
			Assert.AreEqual(bp.getCoordinate(0), 10.0);
			Assert.AreEqual(bp.getCoordinate(1), 20.0);
			Assert.AreEqual(bp.getCoordinate(2), 30.0);
			
			Assert.Throws<Exception>( delegate { bp.setCoordinate(0, -10.0); });
			Assert.Throws<Exception>( delegate { bp.setCoordinate(0, 110.0); });
			Assert.Throws<Exception>( delegate { bp.setCoordinate(1, -10.0); });
			Assert.Throws<Exception>( delegate { bp.setCoordinate(1, 210.0); });
			Assert.Throws<Exception>( delegate { bp.setCoordinate(2, -10.0); });
			Assert.Throws<Exception>( delegate { bp.setCoordinate(2, 310.0); });
			
			IBlauPoint bp2 = bp.clone();
			Assert.AreEqual(bp.CompareTo(bp2), 0);
			Assert.AreEqual(bp2.CompareTo(bp), 0);
			
			int h, h2;
			h = bp.GetHashCode();
			h2 = bp2.GetHashCode();
			Assert.AreEqual(h, h2);
			bp.setCoordinate(0,11.0);
			h = bp.GetHashCode();
			h2 = bp2.GetHashCode();
			Assert.AreNotEqual(h, h2);
			bp2.setCoordinate(0,11.0);
			h = bp.GetHashCode();
			h2 = bp2.GetHashCode();
			Assert.AreEqual(h, h2);
			
			bp.setCoordinate(1,22.0);
			h = bp.GetHashCode();
			h2 = bp2.GetHashCode();
			Assert.AreNotEqual(h, h2);
			bp2.setCoordinate(1,22.0);
			h = bp.GetHashCode();
			h2 = bp2.GetHashCode();
			Assert.AreEqual(h, h2);
			
			bp.setCoordinate(2,33.0);
			h = bp.GetHashCode();
			h2 = bp2.GetHashCode();
			Assert.AreNotEqual(h, h2);
			bp2.setCoordinate(2,33.0);
			h = bp.GetHashCode();
			h2 = bp2.GetHashCode();
			Assert.AreEqual(h, h2);
			
			Dictionary<IBlauPoint, int> dic = new Dictionary<IBlauPoint, int>();
			dic.Add(bp, 1);
			Assert.AreEqual(dic.ContainsKey(bp), true);
			Assert.AreEqual(dic.ContainsKey(bp2), false);
			
			SortedDictionary<IBlauPoint, int> dic2 = new SortedDictionary<IBlauPoint, int>();
			dic2.Add(bp, 1);
			Assert.AreEqual(dic2.ContainsKey(bp), true);
			Assert.AreEqual(dic2.ContainsKey(bp2), true);
		}
		
		[Test()]
		public void BlauSpaceZEROTest()
		{
			Console.WriteLine("BlauSpaceTest");
			int dim = 0;
			string [] names = new string [0];
			double [] mins = new double [0];
			double [] maxs = new double [0];
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			Assert.AreEqual(s.Dimension, 0);
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "Zero dimensional BS => "+s);
		}
		
		[Test()]
		public void BlauSpaceTest()
		{
			Console.WriteLine("BlauSpaceTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			Assert.AreEqual(s.Dimension, 3);
			
			Assert.AreEqual(s.getAxis(0).Name, "x");
			Assert.AreEqual(s.getAxis(1).Name, "y");
			Assert.AreEqual(s.getAxis(2).Name, "z");
			
			Assert.AreEqual(s.getAxisIndex("x")>=0, true);
			Assert.AreEqual(s.getAxisIndex("y")>=0, true);
			Assert.AreEqual(s.getAxisIndex("z")>=0, true);
			
			Assert.AreEqual(s.getAxis(0).MinimumValue, 0.0);
			Assert.AreEqual(s.getAxis(0).MaximumValue, 100.0);
		
			Assert.AreEqual(s.getAxis(1).MinimumValue, 0.0);
			Assert.AreEqual(s.getAxis(1).MaximumValue, 200.0);
			
			Assert.AreEqual(s.getAxis(2).MinimumValue, 0.0);
			Assert.AreEqual(s.getAxis(2).MaximumValue, 300.0);	
			
			Assert.AreEqual(""+s, "BlauSpace[Dim:3]: x(0,100), y(0,200), z(0,300)");	
		}
		
		[Test()]
		public void BlauSpaceRegistryTest()
		{
			Console.WriteLine("BlauSpaceRegistryTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			IBlauSpace s2 = BlauSpace.create(dim, names, mins, maxs);
			IBlauSpace s3 = BlauSpace.create(dim, names, mins, maxs);
			
			Assert.AreSame(s, s2);
			Assert.AreSame(s2, s3);
			Assert.AreSame(s, s3);
			
			
			names = new string [3] {"x", "y", "z1"};
			IBlauSpace X1 = BlauSpace.create(dim, names, mins, maxs);
			Assert.AreNotSame(s, X1);
			
			names = new string [3] {"x", "y", "z"};
			mins = new double [3] {0.0, 0.0, 0.1};
			IBlauSpace X2 = BlauSpace.create(dim, names, mins, maxs);
			Assert.AreNotSame(s, X2);
			
			mins = new double [3] {0.0, 0.0, 0.0};
			maxs = new double [3] {100.0, 200.1, 300.0};
			IBlauSpace X3 = BlauSpace.create(dim, names, mins, maxs);
			Assert.AreNotSame(s, X3);
			
			Assert.AreNotSame(X1, X2);
			Assert.AreNotSame(X2, X3);
			Assert.AreNotSame(X1, X3);
		}
		
		[Test()]
		public void BlauSpaceSerializationTest()
		{
			Console.WriteLine("BlauSpaceSerializationTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace bs = BlauSpace.create(dim, names, mins, maxs);
			
			FileStream fs = new FileStream("bs.xml", FileMode.Create);
			SoapFormatter formatter = new SoapFormatter();
			formatter.Serialize(fs, bs);
    		fs.Flush();
    		fs.Close();
			
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bs => "+bs.ToString());
					
			FileStream fs2 = new FileStream("bs.xml", FileMode.Open);
			BlauSpace bs2 = (BlauSpace)formatter.Deserialize(fs2);
			fs2.Close();
			
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bs2 => "+bs2.ToString());
			
			IBlauSpace bs3 = BlauSpaceRegistry.Instance ().validate(bs2);
			
			Assert.AreEqual(bs==bs3, true);
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bs agrees with validated bs2");
			
			Assert.AreEqual(bs==bs2, true);
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bs agrees with bs2");
		}
		
		[Test()]
		public void BlauSpaceLatticeSerializationTest()
		{
			Console.WriteLine("BlauSpaceLatticeSerializationTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace bs = BlauSpace.create(dim, names, mins, maxs);
			
			int [] steps1 = new int [3] {10,10,10};
			IBlauSpaceLattice bsl1a = BlauSpaceLattice.create(bs, steps1);
			IBlauSpaceLattice bsl1b = BlauSpaceLattice.create(bs, steps1);
			Assert.AreEqual(bsl1a==bsl1b, true);
			
			int [] steps2 = new int [3] {20,20,20};
			IBlauSpaceLattice bsl2 = BlauSpaceLattice.create(bs, steps2);
			Assert.AreEqual(bsl1a==bsl2, false);
			
			FileStream fs = new FileStream("bsl1.xml", FileMode.Create);
			SoapFormatter formatter = new SoapFormatter();
			formatter.Serialize(fs, bsl1a);
    		fs.Flush();
    		fs.Close();
			
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bsl1a => "+bsl1a.ToString());
					
			FileStream fs2 = new FileStream("bsl1.xml", FileMode.Open);
			BlauSpaceLattice bsl1read = (BlauSpaceLattice)formatter.Deserialize(fs2);
			fs2.Close();
			
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bs1read => "+bsl1read.ToString());
			
			IBlauSpaceLattice bsl3 = BlauSpaceLatticeRegistry.Instance ().validate(bsl1read);
			
			Assert.AreEqual(bsl1a==bsl3, true);
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bs1a agrees with validated bs1read");
			
			Assert.AreEqual(bsl1a==bsl1read, true);
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "bs1a agrees with bs1read");
		}
		
		[Test()]
		public void BlauPointSerializationTest()
		{
			Console.WriteLine("BlauSpaceSerializationTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace bs = BlauSpace.create(dim, names, mins, maxs);
			
			SoapFormatter formatter = new SoapFormatter();
			// BinaryFormatter formatter = new BinaryFormatter();
			
			BlauPoint p1 = new BlauPoint(bs);
			BlauPoint p2 = new BlauPoint(bs);
			
			FileStream fs = new FileStream("p1.xml", FileMode.Create);
			formatter.Serialize(fs, p1);
    		fs.Close();
			fs = new FileStream("p2.xml", FileMode.Create);
			formatter.Serialize(fs, p2);
    		fs.Close();
					
			fs = new FileStream("p1.xml", FileMode.Open);
			BlauPoint  p1r = (BlauPoint)formatter.Deserialize(fs);
			fs.Close();
			fs = new FileStream("p2.xml", FileMode.Open);
			BlauPoint  p2r = (BlauPoint)formatter.Deserialize(fs);
			fs.Close();
			
			Assert.AreEqual(p1r.Space==p1.Space, true);
			Assert.AreEqual(p2r.Space==p2.Space, true);
			Assert.AreEqual(p1r.Space==p2r.Space, true);
			Assert.AreEqual(p1.Space==p2.Space, true);
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "All spaces coincides as expected");
		}
		
		[Test()]
		public void QuantizedBlauPointSerializationTest()
		{
			Console.WriteLine("QuantizedBlauPointSerializationTest");
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 200.0, 300.0};
			IBlauSpace bs = BlauSpace.create(dim, names, mins, maxs);
			
			SoapFormatter formatter = new SoapFormatter();
			// BinaryFormatter formatter = new BinaryFormatter();
			
			BlauPoint p1 = new BlauPoint(bs);
			p1.setCoordinate(0, 11.0);
			p1.setCoordinate(1, 21.0);
			p1.setCoordinate(2, 31.0);
			
			BlauPoint p2 = new BlauPoint(bs);
			p2.setCoordinate(0, 21.0);
			p2.setCoordinate(1, 31.0);
			p2.setCoordinate(2, 41.0);
			
			int [] steps = new int[3]; steps[0]=10; steps[1]=20; steps[2]=30;
			IBlauSpaceLattice bsl = BlauSpaceLattice.create(bs, steps);
			
			IBlauPoint qp1 = bsl.quantize (p1);
			IBlauPoint qp2 = bsl.quantize (p2);

			FileStream fs = new FileStream("p1.xml", FileMode.Create);
			formatter.Serialize(fs, p1);
    		fs.Close();
			fs = new FileStream("p2.xml", FileMode.Create);
			formatter.Serialize(fs, p2);
    		fs.Close();
			
			fs = new FileStream("qp1.xml", FileMode.Create);
			formatter.Serialize(fs, qp1);
    		fs.Close();
			fs = new FileStream("qp2.xml", FileMode.Create);
			formatter.Serialize(fs, qp2);
    		fs.Close();
								
			fs = new FileStream("qp1.xml", FileMode.Open);
			IBlauPoint  p1r = (IBlauPoint)formatter.Deserialize(fs);
			fs.Close();
			fs = new FileStream("qp2.xml", FileMode.Open);
			IBlauPoint  p2r = (IBlauPoint)formatter.Deserialize(fs);
			fs.Close();
			
			Assert.AreEqual(p1r.Space==p1.Space, true);
			Assert.AreEqual(p2r.Space==p2.Space, true);
			Assert.AreEqual(p1r.Space==p2r.Space, true);
			Assert.AreEqual(p1.Space==p2.Space, true);
			
			Assert.AreEqual(qp1 is QuantizedBlauPoint, true);
			Assert.AreEqual(qp2 is QuantizedBlauPoint, true);
			
			Assert.AreEqual(qp2.Space==p2.Space, true);
			Assert.AreEqual(qp1.Space==p1.Space, true);
			
			Assert.AreEqual(qp1.CompareTo(p1r), 0);
			Assert.AreEqual(qp2.CompareTo(p2r), 0);
			
			Assert.AreNotEqual(qp1.CompareTo(p1), 0);
			Assert.AreNotEqual(qp2.CompareTo(p2), 0);
			
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "Quantized points are as expected");
		}
		
		
		[Test()]
		public void BlauSpaceAxisTest()
		{
			Console.WriteLine("BlauSpaceAxisTest");
			
			{
				int dim = 3;
				string [] names = new string [3] {"x", "y", "z"};
				double [] mins = new double [3] {10.0, 0.0, 0.0};
				double [] maxs = new double [3] {1.0, 1.0, 1.0};
				IBlauSpace s = null;
				Assert.Throws<Exception>( delegate { s = BlauSpace.create(dim, names, mins, maxs); });
				Console.WriteLine("BlauSpace could not be instantiated: "+(s==null));
			}
		
			{
				int dim = 3;
				string [] names = new string [3] {"x", "y", "z"};
				double [] mins = new double [3] {0.0, 0.0, 0.0};
				double [] maxs = new double [3] {-10.0, 1.0, 1.0};
				IBlauSpace s = null;
				Assert.Throws<Exception>( delegate { s = BlauSpace.create(dim, names, mins, maxs); });
				Console.WriteLine("BlauSpace could not be instantiated: "+(s==null));
			}

			{
				int dim = 3;
				string [] names = new string [3] {"x", "y", "z"};
				double [] mins = new double [3] {0.0, 20.0, 0.0};
				double [] maxs = new double [3] {1.0, 1.0, 1.0};
				IBlauSpace s = null;
				Assert.Throws<Exception>( delegate { s = BlauSpace.create(dim, names, mins, maxs); });
				Console.WriteLine("BlauSpace could not be instantiated: "+(s==null));
			}
			
			{
				int dim = 3;
				string [] names = new string [3] {"x", "y", "z"};
				double [] mins = new double [3] {0.0, 0.0, 0.0};
				double [] maxs = new double [3] {1.0, -20.0, 1.0};
				IBlauSpace s = null;
				Assert.Throws<Exception>( delegate { s = BlauSpace.create(dim, names, mins, maxs); });
				Console.WriteLine("BlauSpace could not be instantiated: "+(s==null));
			}
			
			{
				int dim = 3;
				string [] names = new string [3] {"x", "y", "z"};
				double [] mins = new double [3] {0.0, 0.0, 30.0};
				double [] maxs = new double [3] {1.0, 1.0, 1.0};
				IBlauSpace s = null;
				Assert.Throws<Exception>( delegate { s = BlauSpace.create(dim, names, mins, maxs); });
				Console.WriteLine("BlauSpace could not be instantiated: "+(s==null));
			}
			
			{
				int dim = 3;
				string [] names = new string [3] {"x", "y", "z"};
				double [] mins = new double [3] {0.0, 0.0, 0.0};
				double [] maxs = new double [3] {1.0, 1.0,-30.0};
				IBlauSpace s = null;
				Assert.Throws<Exception>( delegate { s = BlauSpace.create(dim, names, mins, maxs); });
				Console.WriteLine("BlauSpace could not be instantiated: "+(s==null));
			}
		}
		
		[Test()]
		public void BlauSpaceLatticeTest()
		{
			Console.WriteLine("BlauSpaceLatticeTest");
			
			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			int STEPS = 10;
			int[] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;

			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			Assert.AreEqual(bsl.getStepSize(0), 100.0/(double)STEPS);
			Assert.AreEqual(bsl.getStepSize(1), 100.0/(double)STEPS);
			Assert.AreEqual(bsl.getStepSize(2), 100.0/(double)STEPS);
			
			Assert.AreEqual(bsl.getSteps(0), STEPS);
			Assert.AreEqual(bsl.getSteps(1), STEPS);
			Assert.AreEqual(bsl.getSteps(2), STEPS);
			
			IBlauPoint bp = new BlauPoint(s);
			bp.setCoordinate(0, 11.0);
			bp.setCoordinate(1, 22.0);
			bp.setCoordinate(2, 33.0);
			
			IBlauPoint bpq = bsl.quantize(bp);
			Assert.AreNotEqual(bp.CompareTo(bpq), 0);
			
			IBlauPoint bp2 = new BlauPoint(s);
			bp2.setCoordinate(0, 10.0);
			bp2.setCoordinate(1, 20.0);
			bp2.setCoordinate(2, 30.0);
			
			IBlauPoint bpq2 = bsl.quantize(bp2);
			Assert.AreEqual(bp2.CompareTo(bpq2), 0);
			
			Assert.AreEqual(bpq.CompareTo(bp2), 0);
			Assert.AreEqual(bpq.CompareTo(bpq2), 0);
			
			IBlauPoint bp3 = new BlauPoint(s);
			bp3.setCoordinate(0, 9.0);
			bp3.setCoordinate(1, 19.0);
			bp3.setCoordinate(2, 29.0);
			IBlauPoint bpq3 = bsl.quantize(bp3);
			Assert.AreEqual(bpq3.CompareTo(bpq), 0);
			Assert.AreEqual(bpq3.CompareTo(bp2), 0);
			Assert.AreEqual(bpq3.CompareTo(bpq2), 0);
		}
		
		[Test()]
		public void QuantizedBlauPointTest()
		{
			Console.WriteLine("QuantizedBlauPointTest");

			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			int STEPS = 10;
			int[] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;

			IBlauSpaceLattice bsl = BlauSpaceLattice.create(s, STEPSarray);
			IBlauSpaceLattice bsl2 = BlauSpaceLattice.create(s, STEPSarray);
			
			IBlauPoint bp = new QuantizedBlauPoint(s, bsl);
			IBlauPoint bp2 = new QuantizedBlauPoint(s, bsl2);
			IBlauPoint bp3 = new QuantizedBlauPoint(s, bsl);
			bp.setCoordinate(0, 10.0);
			bp.setCoordinate(1, 20.0);
			bp.setCoordinate(2, 30.0);
			bp2.setCoordinate(0, 9.99);
			bp2.setCoordinate(1, 19.99);
			bp2.setCoordinate(2, 29.99);
			bp3.setCoordinate(0, 10.01);
			bp3.setCoordinate(1, 20.01);
			bp3.setCoordinate(2, 30.01);
			Assert.AreEqual(bp.CompareTo(bp2), 0);
			Assert.AreEqual(bp.CompareTo(bp3), 0);
			Assert.AreEqual(bp2.CompareTo(bp), 0);
			Assert.AreEqual(bp.CompareTo(bp), 0);
			Assert.AreEqual(bp3.CompareTo(bp), 0);
			Assert.AreEqual(bp3.CompareTo(bp2), 0);
		}
		
		[Test()]
		public void BlauSpaceIteratorTest()
		{
			Console.WriteLine("BlauSpaceIteratorTest");

			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "=> "+s);
			
			
			int STEPS = 10;
			int[] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;

			IBlauSpaceIterator bsi = new BlauSpaceIterator(s, STEPSarray);
			int count=0;
			int expected=(STEPS+1)*(STEPS+1)*(STEPS+1);
			foreach (IBlauPoint bp in bsi) {
				Assert.IsInstanceOf(typeof(QuantizedBlauPoint), bp);
				SingletonLogger.Instance().DebugLog(typeof(blau_tests), "=> "+bp);
				count++;
			}
			Assert.AreEqual(count, expected);
		}
		
		[Test()]
		public void BlauSpaceIteratorManualTest()
		{
			Console.WriteLine("BlauSpaceIteratorManualTest");

			int dim = 3;
			string [] names = new string [3] {"x", "y", "z"};
			double [] mins = new double [3] {0.0, 0.0, 0.0};
			double [] maxs = new double [3] {100.0, 100.0, 100.0};
			IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
			
			SingletonLogger.Instance().DebugLog(typeof(blau_tests), "=> "+s);
			
			
			int STEPS = 10;
			int[] STEPSarray = new int[s.Dimension]; for (int j=0;j<s.Dimension;j++) STEPSarray[j]=STEPS;

			IBlauSpaceIterator bsi = new BlauSpaceIterator(s, STEPSarray);
			int count=0;
			int expected=(STEPS+1)*(STEPS+1)*(STEPS+1);
			while (bsi.hasNext()) {
				IBlauPoint bp = (IBlauPoint) bsi.next();
				
				Assert.IsInstanceOf(typeof(QuantizedBlauPoint), bp);
				SingletonLogger.Instance().DebugLog(typeof(blau_tests), "=> "+bp);
				count++;
			}
			Assert.AreEqual(count, expected);
		}
	}
}

