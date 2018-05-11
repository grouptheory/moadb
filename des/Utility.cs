using System;

namespace des
{
	public static class Utility
	{
		public static string TimeStamp()
		{
			return DateTime.Now.ToString("-yyyy-MM-dd-hh-mm-ss");
		}

		public static string TimeStamp(string format)
		{
			return DateTime.Now.ToString(format);
		}
	}
}

