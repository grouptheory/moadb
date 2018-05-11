using System;
using args;
using config;

namespace utils
{
	public class Main_utils
	{
		public static void Main (string[] args)
		{
			ApplicationConfig.Initialize();
			
			Console.WriteLine ("Main_utils");
			
			// Command line parsing
	        Arguments CommandLine=new Arguments(args);
	
			bool gaussian = false;
			bool uniform = false;
			bool pointed = false;
			bool pointedimmutable = false;
			bool product = false;
			bool mixture = false;
			bool mixtureimmutable = false;
			bool print = false;
			int matches = 0;
			
	        // Look for specific arguments values and display 
	        // them if they exist (return null if they don't)
	        if (CommandLine["gaussian"] != null) {
				gaussian = Boolean.Parse(CommandLine["gaussian"]);
				matches++;
			}
	        if (CommandLine["uniform"] != null) {
				uniform = Boolean.Parse(CommandLine["uniform"]);
				matches++;
			}
	        if (CommandLine["pointed"] != null) {
				pointed = Boolean.Parse(CommandLine["pointed"]);
				matches++;
			}
			if (CommandLine["pointedimmutable"] != null) {
				pointedimmutable = Boolean.Parse(CommandLine["pointedimmutable"]);
				matches++;
			}
	        if (CommandLine["product"] != null) {
				product = Boolean.Parse(CommandLine["product"]);
				matches++;
			}
	        if (CommandLine["mixture"] != null) {
				mixture = Boolean.Parse(CommandLine["mixture"]);
				matches++;
			}
			if (CommandLine["mixtureimmutable"] != null) {
				mixtureimmutable = Boolean.Parse(CommandLine["mixtureimmutable"]);
				matches++;
			}
	        if (CommandLine["print"] != null) {
				print = Boolean.Parse(CommandLine["print"]);
				matches++;
			}
			
			if (matches==0) {
				Console.WriteLine ("Choose at least one of the following: --gaussian or --uniform or --pointed or --product or --mixture or --print");
			}
			else if (matches > 1) {
				Console.WriteLine ("Choose at most one of the following: --gaussian or --uniform or --pointed or --product or --mixture or --print");
			}
			else {
				if (gaussian) {
					MakeGaussian.MakeGaussian_Main(args);
				}
				else if (uniform) {
					MakeUniform.MakeUniform_Main(args);
				}
				else if (pointed) {
					MakePointed.MakePointed_Main(args);
				}
				else if (pointedimmutable) {
					MakePointedImmutable.MakePointedImmutable_Main(args);
				}
				else if (product) {
					MakeProduct.MakeProduct_Main(args);
				}
				else if (mixture) {
					MakeMixture.MakeMixture_Main(args);
				}
				else if (mixtureimmutable) {
					MakeMixtureImmutable.MakeMixtureImmutable_Main(args);
				}
				else if (print) {
					MakePrintDistribution.MakePrintDistribution_Main(args);
				}
			}
		}
	}
}