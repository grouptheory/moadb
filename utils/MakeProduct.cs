using System;
using System.IO;
using System.Xml.Serialization;    
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using args;
using logger;
using core;
using dist;
using blau;

namespace utils
{
	public class MakeProduct
	{
		public static void MakeProduct_Main (string[] args)
		{
			Console.WriteLine ("MakeProduct");
			
			// Command line parsing
	        Arguments CommandLine=new Arguments(args);
	
			bool err = false;
			string errString = "";
			
			string file1 = "unassigned";
			string file2 = "unassigned";
			string outfile = "unassigned";
						
	        // Look for specific arguments values and display 
	        // them if they exist (return null if they don't)
	        if(CommandLine["file1"] != null) {
				file1 = CommandLine["file1"];
				if ( ! File.Exists(file1) ) {
	            	errString += ("The specified 'file1' was not found: "+file1+"  ");
					err = true;
				}
			}
	        else {
	            errString += ("The 'file1' was not specified.  ");
				err = true;
			}
	
	        if(CommandLine["file2"] != null) {
				file2 = CommandLine["file2"];
				if ( ! File.Exists(file2) ) {
	            	errString += ("The specified 'file2' was not found: "+file2+"  ");
					err = true;
				}
			}
	        else  {
	            errString += ("The 'file2' was not specified.  ");
				err = true;
			}
	
	        if(CommandLine["outfile"] != null) {
				outfile = CommandLine["outfile"];
			}
	        else  {
	            errString += ("The 'outfile' was not specified.  ");
				err = true;
			}
	
			if (err) {
		        Console.Out.WriteLine("Arguments parsing failed.");
		        Console.Out.WriteLine("  "+errString);
			}
			else {
		        Console.Out.WriteLine("Arguments parsing successful.");
		        Console.Out.WriteLine("  file1 = "+file1);
		        Console.Out.WriteLine("  file2 = "+file2);
		        Console.Out.WriteLine("  outfile = "+outfile);
				
				SoapFormatter formatter = new SoapFormatter();
				
				FileStream fs = new FileStream(file1, FileMode.Open);
				IDistribution d1 = (IDistribution)formatter.Deserialize(fs);
				fs.Close();
				
				fs = new FileStream(file2, FileMode.Open);
				IDistribution d2 = (IDistribution)formatter.Deserialize(fs);
				fs.Close();
			
				if (BlauSpace.intersects(d1.SampleSpace, d2.SampleSpace)) {
		        	Console.Out.WriteLine("The sample spaces of the two distributions have a non-empty intersection.");
					Console.Out.WriteLine("  d1: "+d1);
					Console.Out.WriteLine("  d2: "+d2);
		        	Console.Out.WriteLine("The product must be constructed over disjoint sample spaces.");
					Console.Out.WriteLine("This is a fatal error, preventing the construction of the product distribution.");
				}
				else {
					int dim3 = d1.SampleSpace.Dimension + d2.SampleSpace.Dimension;
					string [] names3 = new string [dim3];
					double [] mins3 = new double [dim3];
					double [] maxs3 = new double [dim3];
					for (int i=0;i<d1.SampleSpace.Dimension;i++) {
						names3[i]=d1.SampleSpace.getAxis(i).Name;
						mins3[i]=d1.SampleSpace.getAxis(i).MinimumValue;
						maxs3[i]=d1.SampleSpace.getAxis(i).MaximumValue;
					}
					for (int i=0;i<d2.SampleSpace.Dimension;i++) {
						names3[d1.SampleSpace.Dimension+ i]=d2.SampleSpace.getAxis(i).Name;
						mins3[d1.SampleSpace.Dimension+ i]=d2.SampleSpace.getAxis(i).MinimumValue;
						maxs3[d1.SampleSpace.Dimension+ i]=d2.SampleSpace.getAxis(i).MaximumValue;
					}
					IBlauSpace s3 = BlauSpace.create(dim3, names3, mins3, maxs3);
					
					Console.Out.WriteLine("  product space: "+s3);
					
					Product d3 = new Product(s3);
					d3.Add(d1);
					d3.Add(d2);
					d3.DistributionComplete();
				
					Console.Out.WriteLine("Distribution: "+d3);
					
					
					fs = new FileStream(outfile, FileMode.Create);
					formatter.Serialize(fs, d3);
		    		fs.Close();
				}
			}
		}
	}
}

