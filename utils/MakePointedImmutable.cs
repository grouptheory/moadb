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
	public class MakePointedImmutable
	{
		public static void MakePointedImmutable_Main (string[] args)
		{
			Console.WriteLine ("MakePointedImmutable");
			
			// Command line parsing
			Arguments CommandLine=new Arguments(args);
			
			bool err = false;
			string errString = "";
			
			double val, min, max;
			string variable = "unassigned";
			string outfile = "unassigned";
			
			val = min = max = -1.0;
			
			// Look for specific arguments values and display 
			// them if they exist (return null if they don't)
			if(CommandLine["value"] != null) {
				try {
					val = Double.Parse (CommandLine["value"]);
					min = val;
					max = val;
				}
				catch (Exception) {
					errString += ("The specified 'value' was not valid.  ");
					err = true;
				}
			}
			else {
				errString += ("The 'value' was not specified.  ");
				err = true;
			}
			
			if (CommandLine["variable"] != null) {
				variable = CommandLine["variable"];
			}
			else  {
				errString += ("The 'variable' was not specified.  ");
				err = true;
			}
			
			if(CommandLine["min"] != null) {
				try {
					min = Double.Parse (CommandLine["min"]);
				}
				catch (Exception) {
					errString += ("The specified 'min' was not valid.  ");
					err = true;
				}
			}
			else {
				errString += ("The 'min' was not specified.  ");
				err = true;
			}
			
			if(CommandLine["max"] != null) {
				try {
					max = Double.Parse (CommandLine["max"]);
				}
				catch (Exception) {
					errString += ("The specified 'max' was not valid.  ");
					err = true;
				}
			}
			else  {
				errString += ("The 'max' was not specified.  ");
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
				Console.Out.WriteLine("  value = "+val);
				Console.Out.WriteLine("  variable = "+variable);
				Console.Out.WriteLine("  min = "+min);
				Console.Out.WriteLine("  max = "+max);
				Console.Out.WriteLine("  outfile = "+outfile);
				
				int dim = 1;
				string [] names = new string [1] {""};
				double [] mins = new double [1] {0.00};
				double [] maxs = new double [1] {0.0};
				names[0] = variable;
				mins[0] = min;
				maxs[0] = max;
				IBlauSpace s = BlauSpace.create(dim, names, mins, maxs);
				IDistribution d = new Distribution_Pointed_Immutable(s, val);
				
				Console.Out.WriteLine("Distribution: "+d);
				
				SoapFormatter formatter = new SoapFormatter();
				
				FileStream fs = new FileStream(outfile, FileMode.Create);
				formatter.Serialize(fs, d);
				fs.Close();
			}
		}
	}
}

