using System;
using System.Collections.Generic;
using System.Text;

namespace logger
{
    public abstract class AbstractObserver : IObserver
    {
        protected string _key;

        protected EventHandler<LogEventArgs> _handler;

        public AbstractObserver(string key)
        {
            _key = key;
            _handler = new EventHandler<LogEventArgs>(logger_LogRequest);
        }

        public string Key
        {
            get { return _key; }
        }

        
        #region IObserver Members

        public abstract string Name { get; }

        public void Register(ILog logger)
        {
            logger.LogRequest += _handler;
        }

        public void Unregister(ILog logger)
        {
            logger.LogRequest -= _handler;
        }


        #endregion

        protected abstract void logger_LogRequest(object sender, LogEventArgs e);

        /// <summary>
        /// Formats a LogEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected static string FormatMessage(object sender, LogEventArgs e)
        {
            string msg = FormatMessage(sender, e, false);
            return msg;
            //string senderType = string.Empty;

            //if ((e == null) || (sender == null))
            //{
            //    senderType = "NULL";
            //}
            //else if (e.Sender is Type)  // Static Methods should send a type object as the 'sender'
            //{
            //    senderType = e.Sender.ToString();
            //}
            //else
            //{
            //    senderType = e.Sender.GetType().ToString();
            //}


            ///* Format Key:
            // * 0:  DateStamp
            // * 1:  TimeStamp
            // * 2:  Log Level
            // * 3:  Key
            // * 4:  Sender Type
            // * 5:  Message
            // */
            //string format = @"{0} {1} ({2}/{3}/{4}):  {5}";

            //string message = string.Format(format,
            //    DateTime.Now.ToShortDateString(),   //0
            //    DateTime.Now.ToShortTimeString(),   //1
            //    e.LogLevel,                         //2
            //    e.Key,                              //3
            //    senderType,                         //4
            //    e.Message                           //5
            //);


            //if (e.Exception != null)
            //{
            //    message += Environment.NewLine + e.Exception.ToString();
            //}

            //return message;
        }


        /// <summary>
        /// Formats a LogEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected static string FormatMessage(object sender, LogEventArgs e, bool includeDateTime)
        {
            string msg = string.Empty;
            string dateTime = string.Empty;

            if (includeDateTime)
            {
                string dtFormat = "{0} {1} ";
                dateTime = string.Format(dtFormat, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
         
            msg = FormatMessage(sender, e, dateTime);

            return msg;
        }

        protected static string FormatMessage(object sender, LogEventArgs e, string dateTime)
        {

            string senderType = string.Empty;

            if ((e == null) || (sender == null) || (e.Sender == null))
            {
                senderType = "NULL";
            }
            else if (e.Sender is Type)  // Static Methods should send a type object as the 'sender'
            {
                senderType = e.Sender.ToString();
            }
            else
            {
                senderType = e.Sender.GetType().ToString();
            }

            /* Format Key:
             * 0:  DateTime
             * 1:  Log Level
             * 2:  Key
             * 3:  Sender Type
             * 4:  Message
             */
            string format = @"{0}({1}/{2}/{3}):  {4}";

            string message = string.Format(format,
                dateTime,       //0
                e.LogLevel,     //1
                e.Key,          //2
                senderType,     //3
                e.Message       //4
            );


            if (e.Exception != null)
            {
                message += Environment.NewLine + e.Exception.ToString();
            }

            return message;
        }
		
		
		private Dictionary<object, LogLevel> _thresholds = new Dictionary<object, LogLevel>();
		
		public void SetThreshold(object sender, LogLevel lvl) {

			// Console.WriteLine ("GGG SetThreshold");

			if (_thresholds.ContainsKey(sender)) {
				_thresholds.Remove(sender);
			}
			_thresholds.Add (sender,lvl);
		}
		
		private LogLevel _defaultLevel = LogLevel.Warning;
		
		public LogLevel DEFAULT_THRESHOLD {
			get { return _defaultLevel; }
			set { _defaultLevel = value; }
		}
		
		private LogLevel GetThreshold(object sender) {
			if (!_thresholds.ContainsKey(sender)) {
				return DEFAULT_THRESHOLD;
			}
			return _thresholds[sender];
		}
		
		protected bool GetsThrough(object sender, LogLevel msglvl) {
			
			// Console.WriteLine ("GGG GetThreshold(sender) "+GetThreshold(sender));
			// Console.WriteLine ("GGG msglvl "+msglvl);

			return GetThreshold(sender)<=msglvl;
		}
    }
}
