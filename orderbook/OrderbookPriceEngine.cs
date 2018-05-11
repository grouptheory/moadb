using System;
using core;

namespace orderbook
{
	public class OrderbookPriceEngine : IOrderbookPriceEngine
	{
		private static double MU = 0.0;
		private static double SIGMA = 0.5;
		private static double DEFAULT_MARKET_PADDING_PROBABILITY = 0.01;

		private static double MAX_STEADYSPREAD = 0.10;
		private static double MIN_STEADYSPREAD = 0.02;
		private static double DEFAULT_STEADY_SPREAD_TO_PRICE_RATIO = 1.0/245.0;
		
		private static double DEFAULT_MODE_TO_PRICE_RATIO = 1.01;
		private static double DEFAULT_MARKET_ORDER_FRACTION = 1.0/8.0;

		public OrderbookPriceEngine()
		{
			MarketProb = DEFAULT_MARKET_ORDER_FRACTION;
			SteadySpreadToPriceRatio = DEFAULT_STEADY_SPREAD_TO_PRICE_RATIO;
			ModeToPriceRatio = DEFAULT_MODE_TO_PRICE_RATIO;
		}

		private void processOrderbook (IOrderbook_Agent ob)
		{
			Price = ob.getPrice ();
		}

		private double _P = -1.0;
		public double Price {
			get { return _P; }
			set { 
				_P = value; 
				_SteadySpread = SteadySpreadToPriceRatio * Price;
				if (_SteadySpread > MAX_STEADYSPREAD) _SteadySpread = MAX_STEADYSPREAD;
				if (_SteadySpread < MIN_STEADYSPREAD) _SteadySpread = MIN_STEADYSPREAD;
				XSCALE = (ModeToPriceRatio * Price - Price - _SteadySpread/2.0) / (RAW_MODE);
			}
		}

		private double _SteadySpreadToPriceRatio = DEFAULT_STEADY_SPREAD_TO_PRICE_RATIO;

		private double _SteadySpread = MIN_STEADYSPREAD;

		public double SteadySpreadToPriceRatio {
			get { return _SteadySpreadToPriceRatio; }
			set { 
				_SteadySpreadToPriceRatio = value; 
				_SteadySpread = SteadySpreadToPriceRatio * Price;
				if (_SteadySpread > MAX_STEADYSPREAD) _SteadySpread = MAX_STEADYSPREAD;
				if (_SteadySpread < MIN_STEADYSPREAD) _SteadySpread = MIN_STEADYSPREAD;
				XSCALE = (ModeToPriceRatio * Price - Price - _SteadySpread/2.0) / (RAW_MODE);
			}
		}

		private double _MarketPadding;

		private double _ModeToPriceRatio = DEFAULT_MODE_TO_PRICE_RATIO;
		public double ModeToPriceRatio {
			get { return _ModeToPriceRatio; }
			set { 
				_ModeToPriceRatio = value;
				_MarketPadding = CDFinv(DEFAULT_MARKET_PADDING_PROBABILITY);
				XSCALE = (ModeToPriceRatio * Price - Price - _SteadySpread/2.0) / (RAW_MODE);
			}
		}

		private double _MarketProb = DEFAULT_MARKET_ORDER_FRACTION;
		public double MarketProb {
			get { return _MarketProb; }
			set { _MarketProb = value; }
		}

		private double RAW_MODE = Math.Exp(MU - Math.Pow (SIGMA, 2.0));
		private double XSCALE;

		public bool isMarket (IOrderbook_Agent ob)
		{
			processOrderbook (ob);
			return SingletonRandomGenerator.Instance.NextDouble () <= MarketProb;
		}
		
		public double getMarketPriceOffset (IOrderbook_Agent ob)
		{
			processOrderbook (ob);
			return SingletonRandomGenerator.Instance.NextUniform(-_SteadySpread/2.0 - _MarketPadding, 
			                                                     +_SteadySpread/2.0 + _MarketPadding);
		}

		public double getAskPriceOffset(IOrderbook_Agent ob) {
			processOrderbook (ob);
			double y = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
			return _SteadySpread/2.0 + XSCALE * CDFinv(y);
		}
		
		public double getBidPriceOffset(IOrderbook_Agent ob) {
			processOrderbook (ob);
			double y = SingletonRandomGenerator.Instance.NextUniform (0.0, 1.0);
			return - _SteadySpread/2.0 - XSCALE * CDFinv(y);
		}

		public double getMarketPrice (IOrderbook_Agent ob)
		{
			processOrderbook (ob);
			return Price + getMarketPriceOffset(ob); 
		}

		public double getAskPrice (IOrderbook_Agent ob)
		{
			processOrderbook (ob);
			if (isMarket(ob)) return getMarketPrice(ob);
			else return Price + getAskPriceOffset(ob); 
		}
		
		public double getBidPrice (IOrderbook_Agent ob)
		{
			processOrderbook (ob);
			if (isMarket(ob)) return getMarketPrice(ob);
			else return Price + getBidPriceOffset (ob);
		}
		
		private static double [] coeff = 
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
		
		private static double[] tpower = new double[10];
		
		private static double erf (double x)
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
		
		private static double CDF(double x)
		{
			return 0.5 + 0.5 * erf ( (Math.Log(x) - MU) / Math.Sqrt(2.0*SIGMA*SIGMA) );
		}
		
		private static double CDFinv(double y)
		{
			if (!CDF_lut_initialized) fill_CDFinv_lut();
			int yi = (int)(y * (double)YGRID);
			return CDFinv_lut[yi];
		}
		
		static bool CDF_lut_initialized = false;
		const int YGRID=10000;
		const double XSTEP=0.00001;
		static double[] CDFinv_lut = new double [YGRID];
		
		private static void fill_CDFinv_lut ()
		{
			for (int i=0; i<YGRID; i++) {
				CDFinv_lut[i] = -1.0;
			}
			
			for (double x=XSTEP; ; x+=XSTEP) {
				double y = CDF (x);
				int yi = (int)(y * (double)YGRID);
				if (CDFinv_lut[yi] < 0.0) {
					CDFinv_lut[yi] = x;
					if (yi == YGRID-1) break;
				}
			}
			
			CDF_lut_initialized = true;
		}
	}
}

