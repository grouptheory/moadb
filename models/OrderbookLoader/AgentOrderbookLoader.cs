using System;
using System.IO;
using System.Collections.Generic;
using core;
using agent;
using logger;
using des;

namespace models
{
	public class AgentOrderbookLoader : AbstractAgent, IAgent_NonParticipant
	{		
		protected override string BASENAME {
			get { return "AgentOrderbookLoader"; }
		}
		
		private string _path;
		
		public AgentOrderbookLoader(string path, IBlauPoint coordinates, IAgentFactory creator, int id) : base(coordinates, creator, id, 0.0) { 
			_drain = false;
			_path = path;
			_shots = 0;
		}
		
		public override void FilledOrderNotification(IOrder filledOrder, double price, int volume)         { }
		public override void PartialFilledOrderNotification(IOrder partialOrder, double price, int volume) { }
		
		public override void SimulationStartNotification(IPopulation pop) { 
			SingletonLogger.Instance().DebugLog(typeof(AgentOrderbookLoader), "*** AgentOrderbookLoader got ISimulationStart @ "+Scheduler.GetTime());
		}
		public override void SimulationEndNotification() { 
			SingletonLogger.Instance().DebugLog(typeof(AgentOrderbookLoader), "*** AgentOrderbookLoader got ISimulationEnd @ "+Scheduler.GetTime());
			SingletonLogger.Instance().InfoLog(typeof(AgentOrderbookLoader), "*** OB "+Orderbook.ToStringLong());
		}
		
		private int _shots;
		
		protected override double GetTimeToNextActionPrompt() { 
			SingletonLogger.Instance().DebugLog(typeof(AgentOrderbookLoader), "*** AgentOrderbookLoader GetTimeToNextActionPrompt _shots="+_shots+" @ "+Scheduler.GetTime());
			if (_shots == 0) {
				_shots++;
				return 0.0; 
			}
			else if (_shots == 1) {
				_shots++;
				parseCSV();
				return 2.0; 
			}
			else {
				// SingletonLogger.Instance().ErrorLog(typeof(AgentOrderbookLoader), "AgentOrderbookLoader: Scheduler breakdown detected... Double.MaxValue time has passed");
				return 2.0; 
			}
		}
		
		private readonly static double DecideToAct_PROBABILITY = 0.50;
		protected override bool DecideToAct() { 
			if (_shots<2) return false; 
			else {
			return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToAct_PROBABILITY);
			}
		}
		
		private bool _drain;
		public bool Drain {
			get { return _drain; }
			set { _drain = value; }
		}
				
		private readonly static double DecideToCancelOpenOrder_PROBABILITY = 0.05;
		protected override bool DecideToCancelOpenOrder(IOrder openOrder) { 
			if (_drain) {
				return (SingletonRandomGenerator.Instance.NextDouble() <= DecideToCancelOpenOrder_PROBABILITY);
			}
			else {
				return false;
			}
		}
		
		protected override bool DecideToMakeOrder()                       { return false; }
		protected override bool DecideToSubmitBid()                       { return false; }
		protected override double GetBidPrice()                           { return 1.0; }
		protected override int GetBidVolume()                             { return 1; }
		protected override double GetAskPrice()                           { return 1.0; }
		protected override int GetAskVolume()                             { return 1; }

		private void parseCSV()
		{
			try {
				using (StreamReader readFile = new StreamReader(_path)) {
					string line;
					string[] row;
					int rownum=1;
					while ((line = readFile.ReadLine()) != null)
					{
						row = line.Split(',');
						
						if (row.Length != 6) {
							throw new Exception("Bad row #"+rownum+" has "+row.Length+" fields: "+line);
						}
						
						/*
						string timeString = row[0];
						string priceString = row[1];
						double price = Double.Parse (priceString);
						*/
						
						string directionString = row[2];
						bool isBid = (directionString.Equals("Bid") ? true : false);
						
						string orderpriceString = row[3];
						double orderprice = Double.Parse (orderpriceString);
						
						string ordersizeString = row[4];
						int ordersize = Int32.Parse (ordersizeString);
						
						IOrder order;
						if (isBid) {
							order = Orderbook.addBid(orderprice, ordersize, this);
						}
						else {
							order = Orderbook.addAsk(orderprice, ordersize, this);
						}
						
						SingletonLogger.Instance().InfoLog(typeof(AgentOrderbookLoader), "AgentOrderbookLoader placed: "+order);
						
						AddToOpenOrderList(order);
						
						rownum++;
						
					}
				}
			}
			catch (Exception e) {
		    	// show
				SingletonLogger.Instance().ErrorLog(typeof(AgentOrderbookLoader), "Error in parseCSV -- "+e.Message);
				throw new Exception("Error in parseCSV -- "+e.Message);
		  	}
		}
	}
}

