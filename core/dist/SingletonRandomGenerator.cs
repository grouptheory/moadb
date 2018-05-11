using System;

namespace core
{
	public class SingletonRandomGenerator
	{
		public static IRandomGenerator Instance
		{
			get 
			{
				return RandomGenerator.Instance;
			}
		}
	}
}

