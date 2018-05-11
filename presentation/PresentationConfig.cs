using System;

namespace presentation
{
	public class PresentationConfig
	{
		private static string _dir;
		public static string Directory {
			get { return _dir; }
			set { _dir = value; }
		}
	}
}

