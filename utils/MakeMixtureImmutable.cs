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
	public class MakeMixtureImmutable
	{
		public static void MakeMixtureImmutable_Main (string[] args)
		{
			Console.WriteLine ("MakeMixtureImmutable");
			
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
			
			double weight1=-1.0;
			// Look for specific arguments values and display 
			// them if they exist (return null if they don't)
			if(CommandLine["weight1"] != null) {
				try {
					weight1 = Double.Parse (CommandLine["weight1"]);
					if ((weight1<0.0) || (weight1>1.0)) {
						errString += ("The specified 'weight1' was not in the range [0,1].  ");
						err = true;
					}
				}
				catch (Exception) {
					errString += ("The specified 'weight1' was not valid.  ");
					err = true;
				}
			}
			else {
				errString += ("The 'weight1' was not specified.  ");
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
				Console.Out.WriteLine("  weight1 = "+weight1);
				Console.Out.WriteLine("  file2 = "+file2);
				Console.Out.WriteLine("  weight2 = "+(1-weight1));
				Console.Out.WriteLine("  outfile = "+outfile);
				
				SoapFormatter formatter = new SoapFormatter();
				
				FileStream fs = new FileStream(file1, FileMode.Open);
				IDistribution d1 = (IDistribution)formatter.Deserialize(fs);
				fs.Close();
				
				fs = new FileStream(file2, FileMode.Open);
				IDistribution d2 = (IDistribution)formatter.Deserialize(fs);
				fs.Close();
				
				if (!BlauSpace.contains(d1.SampleSpace, d2.SampleSpace) || !BlauSpace.contains(d2.SampleSpace, d1.SampleSpace)) {
					Console.Out.WriteLine("The sample spaces of the two distributions are not identical intersection.");
					Console.Out.WriteLine("  d1: "+d1);
					Console.Out.WriteLine("  d2: "+d2);
					Console.Out.WriteLine("The mixture must be constructed over identical sample spaces.");
					Console.Out.WriteLine("This is a fatal error, preventing the construction of the mixture distribution.");
				}
				else {
					int dim3 = d1.SampleSpace.Dimension;
					string [] names3 = new string [dim3];
					double [] mins3 = new double [dim3];
					double [] maxs3 = new double [dim3];
					for (int i=0;i<d1.SampleSpace.Dimension;i++) {
						names3[i]=d1.SampleSpace.getAxis(i).Name;
						mins3[i]=d1.SampleSpace.getAxis(i).MinimumValue;
						maxs3[i]=d1.SampleSpace.getAxis(i).MaximumValue;
					}
					IBlauSpace s3 = BlauSpace.create(dim3, names3, mins3, maxs3);
					
					Mixture_Immutable d3 = new Mixture_Immutable(s3);
					d3.Add(d1, weight1);
					d3.Add(d2, 1.0-weight1);
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

