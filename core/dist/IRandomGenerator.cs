using System;

namespace core
{
	public interface IRandomGenerator
	{
		int? Seed {
			get; 
			set;
		}
			
		bool NextBoolean();
		int Next();
		int Next(int maxValue);
		int Next(int minValue, int maxValue);
		void NextBytes(byte[] buffer);
		double NextDouble();
		int NextPoisson(double lambda);
		int SafePossionMaxLambda
		{
			get;
			set;
		}
		
		double NextSafePoisson(double lambda);
		double NextUniform(double a, double b);
		double NextLogNormal(double mean, double stddev);
		double NextGaussian(double mean, double stddev);
		double NextGaussianPositive(double mean, double stddev);
		
		void realize();
	}
}

