using System;

namespace core
{
	class RandomGenerator : IRandomGenerator
	{
		private static RandomGenerator _instance;
		private static Random _random;
		private int? _seed = null;
		private int _safePossionMaxLambda = 40;
		
		private RandomGenerator()
		{
			if(_random == null) {_random = new Random();}
		}
		
		public void realize() {
			// no op
		}
		
		public int? Seed
		{
			//using Instance not local ref _instance
			get{
				Instance.realize();
				return _instance._seed;
			}
			set
			{
				Instance.realize();
				if(_instance._seed != value)
				{
					_instance._seed = value;
					if(_instance._seed == null)
					{
						_instance._seed = (int?) GetEpochTime();
						Console.WriteLine("RandomGenerator.Seed is set to "+_instance._seed);
					} 
					else {
						if (_instance._seed < 0) {
							_instance._seed = (int?) GetEpochTime();
							Console.WriteLine("RandomGenerator.Seed is set to "+_instance._seed);
						}
						else {
							Console.WriteLine("RandomGenerator.Seed set to "+_instance._seed);
						}
					}
					
					//seed is not null, it has an int value
					_random = new Random(_instance._seed.Value);
				}
			}
		}
			
		private static long GetEpochTime() 
		{ 
			DateTime dtCurTime = DateTime.Now; 
			DateTime dtEpochStartTime = Convert.ToDateTime("1/1/1970 8:00:00 AM"); 
			TimeSpan ts = dtCurTime.Subtract(dtEpochStartTime); 
			
			long epochtime; 
			epochtime = ((((((ts.Days * 24) + ts.Hours) * 60) + ts.Minutes) * 60) + ts.Seconds); 
			return epochtime; 
		} 

		public static IRandomGenerator Instance
		{
			get 
			{
				if(_instance == null) {_instance = new RandomGenerator();}
				return _instance;
			}
		}

		public bool NextBoolean() {
			return Next()%2 == 0;
		}

		public int Next()
		{
			return _random.Next();
		}
		
		public int Next(int maxValue)
		{
			return _random.Next(maxValue);
		}
		
		public int Next(int minValue, int maxValue)
		{
			return _random.Next(minValue, maxValue);
		}

		public void NextBytes(byte[] buffer)
		{
			_random.NextBytes(buffer);
		}
		
		public double NextDouble()
		{
			return _random.NextDouble();
		}
		
		public int NextPoisson(double lambda)
		{
			int k = 0;
			double p = 1.0;
			double L = Math.Exp(- lambda);
			do
			{
				k++;
				p *= _random.NextDouble();
			}while (p >= L);
			
			return k-1;
		}

		public int SafePossionMaxLambda
		{
			get{return _safePossionMaxLambda;}
			set{_safePossionMaxLambda = value;}
		}
		
		public double NextSafePoisson(double lambda)
		{
			double safePoisson;
			if (lambda > _safePossionMaxLambda)
			{
				//lambda value may cause a continous loop, use Gaussian
				double mean = lambda;
				double stddev = Math.Sqrt(lambda);
				
				safePoisson = NextGaussian(mean, stddev);
			}
			else
			{
				safePoisson = NextPoisson(lambda);
			}
			
			return safePoisson;
		}

		public double NextUniform(double a, double b)
		{
			double u = a + _random.NextDouble() * (b - a);
			return u;
		}
		
		public double NextLogNormal(double mean, double stddev)
		{
			return Math.Exp ( NextGaussian(mean, stddev) );
		}

		double [] coeff = 
		  { -1.26551223,
			+1.00002368,
			+0.37409196,
			+0.09678418,
			-0.18628806,
			+0.27886807,
			-1.13520398,
			+1.48851587,
			-0.82215223,
			+0.17087277 };

		double[] tpower = new double[10];

		private double erf (double x)
		{
			double t = 1.0 / (1.0 + 0.5 * Math.Abs (x));

			tpower[0]=1.0;
			for (int i=1; i<10; i++) {
				tpower[i] = tpower[i-1]*t;
			}

			double accum=0.0;
			for (int i=0; i<10; i++) {
				accum += coeff[i] * tpower[i];
			}
			double tau = t * Math.Exp (-x*x + accum);

			if (x >= 0.0) {
				return 1.0 - tau;
			} else {
				return tau - 1.0;
			}
		}

		public double NextGaussian(double mean, double stddev)
		{
			double r,x,y;
			do
			{
				x = NextUniform(-1.0, 1.0);
				y = NextUniform(-1.0, 1.0);
				r = (x*x)+(y*y);
			}
			while ((r >= 1) || (r == 0));
			double g = mean + stddev * (x * Math.Sqrt(-2 * Math.Log(r) /r));
			return g;
		}
		
		
		public double NextGaussianPositive(double mean, double stddev)
		{
			double v=0.0;
			do
			{
				v = this.NextGaussian(mean, stddev);
			}
			while (v <= 0.0);
			return v;
		}
		
		public override string ToString ()
		{
			string s = "RandomGenerator (Seed="+Seed+")";
			return s;
		}
	}
}

