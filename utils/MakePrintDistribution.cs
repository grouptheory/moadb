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
	public class MakePrintDistribution
	{
		public static void MakePrintDistribution_Main (string[] args)
		{
			Console.WriteLine ("MakePrintDistribution");
			
			// Command line parsing
	        Arguments CommandLine=new Arguments(args);
	
			bool err = false;
			string errString = "";
			
			string file1 = "unassigned";
						
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
	
			if (err) {
		        Console.Out.WriteLine("Arguments parsing failed.");
		        Console.Out.WriteLine("  "+errString);
			}
			else {
		        Console.Out.WriteLine("Arguments parsing successful.");
		        Console.Out.WriteLine("  file1 = "+file1);
				
				SoapFormatter formatter = new SoapFormatter();
				
				FileStream fs = new FileStream(file1, FileMode.Open);
				IDistribution d1 = (IDistribution)formatter.Deserialize(fs);
				fs.Close();
				
				Console.Out.WriteLine("The distribution in "+file1+" is:");
				Console.Out.WriteLine("");
				Console.Out.WriteLine(""+d1);
				Console.Out.WriteLine("");
			}
		}
	}
}

