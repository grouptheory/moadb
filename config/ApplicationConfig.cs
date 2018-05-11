using System;
using System.IO;

namespace config
{
	public class ApplicationConfig
	{
		public static string EXECDIR {
			get { return CURRENT_EXECDIR_VALUE; }
		}
		
		public static string ROOTDIR {
			get { return ORIGINAL_EXECDIR_VALUE; }
		}

		public static readonly string ORIGINAL_EXECDIR_VALUE = "/Users/human/Documents/dev/moadb/moadb/_execution_/";
		private static string CURRENT_EXECDIR_VALUE = ORIGINAL_EXECDIR_VALUE;
		
		public static void Initialize() {
			string dir = EXECDIR;
			try
	  		{
		  		//Set the current directory.
		  		Directory.SetCurrentDirectory(dir);
	  		}
	  		catch (DirectoryNotFoundException e)
	  		{
		  		Console.WriteLine("The specified EXECDIR directory "+EXECDIR+" does not exist. {0}", e);
	  		}
		}


		public static void Initialize(string subdir) {
			string dir = Path.Combine(ORIGINAL_EXECDIR_VALUE, subdir);
			CURRENT_EXECDIR_VALUE = dir;

			try
			{
				//Set the current directory.
				Directory.SetCurrentDirectory(dir);
			}
			catch (DirectoryNotFoundException e)
			{
				Console.WriteLine("The specified directory "+dir+" does not exist. {0}", e);
			}
		}

		
		public static string InitializeAtCurrentWorkingDirectory() {
			string dir = ORIGINAL_EXECDIR_VALUE;
			string module = "";
			try
			{
				//Set the current directory.
				dir = Directory.GetCurrentDirectory();

				string stripped = ORIGINAL_EXECDIR_VALUE.Substring(0,ORIGINAL_EXECDIR_VALUE.Length-1);


				if (dir.LastIndexOf(stripped)!=0) {
					throw new Exception("Program was run in "+stripped+" which is outside of moadb root: "+ORIGINAL_EXECDIR_VALUE);
				}

				if (dir.Length > ORIGINAL_EXECDIR_VALUE.Length) {
					module = dir.Substring (ORIGINAL_EXECDIR_VALUE.Length);
					Initialize(module+"/");
				}
				else {
					Initialize();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to get Directory.GetCurrentDirectory(): {0}", e);
				throw e;
			}

			return module;
		}
	}
}

