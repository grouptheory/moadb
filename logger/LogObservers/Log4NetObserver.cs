//MdS 8/25/08
//-----------
using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.IO;
using log4net.Config;
using System.Configuration;

namespace logger
{
    /// <summary>
    /// The default observer log data throw log4net frame work
    /// </summary>
    public class Log4NetObserver : AbstractObserver
    {	
        log4net.ILog _logger;
        /// <summary>
        /// construct a logger listiener
        /// </summary>
        /// <param name="logConfigFile">log4net configuration file</param>
        public Log4NetObserver(string logConfigFile)
            : this(logConfigFile, "") { }

        /// <summary>
        /// construct a logger listiener that logs only data associated with the key value
        /// </summary>
        /// <param name="logConfigFile">log4net configuration file</param>
        /// <param name="key">key: identify which messages to be logged</param>
        public Log4NetObserver(string logConfigFile, string key)
            : base(key)
        {
            InitLogger(logConfigFile);
        }

        /// <summary>
        /// initialize the logger
        /// </summary>
        /// <param name="logConfigFile"></param>
        public void InitLogger(string logConfigFile)
        {
            //Console.WriteLine("Log4net InitLogger");
			
            if (logConfigFile == null)
                return;

            //Loading logger config without forcing a schema.
            FileInfo logFI = new FileInfo(logConfigFile);
			//[Resolved in System ref]Disabled or a System.Uri ref error:  

            log4net.Config.XmlConfigurator.Configure(logFI);
			
			_logger = LogManager.GetLogger(Name);

            if (_logger != null)
            {
                _logger.Debug("=========================================================================================");
                _logger.Debug("===  MOADB Execution: "+System.DateTime.Now);
                _logger.Debug("=========================================================================================");
                _logger.Debug("Log4net is enabled.");
            }
            else
            {
                Console.WriteLine("Log4net is null.. Please enable console log observer");
            }
        }

        /// <summary>
        /// identify the logger name in the log4net config file
        /// </summary>
        public override string Name
        {
            get { return ""; }
        }


        protected override void logger_LogRequest (object sender, LogEventArgs e)
		{
			// Console.WriteLine ("GGG logger_LogRequest: " + e.Message);

			if ((this._key != "") && (this._key != e.Key)) {
				// Console.WriteLine ("GGG lIGNORING");
				//ignore message
				return;
			}
			
			if (GetsThrough (e.Sender, e.LogLevel)) {
				// Console.WriteLine ("GGG Get through");
				string message = FormatMessage (sender, e, false);

				Console.WriteLine (""+message);

				// Log4Net is crap.
				// Log (message, e);
			} else {
				// Console.WriteLine ("GGG Not Get through");
			}
        }
		
        /*
         * Removed this when we provided an overload for base.FormatMessage
         * that will return a message without any date time stamps
         * Josh & Mohamed
         * November 29, 2007
         */

        //private string RemovePrefixDate(string message)
        //{
        //    int amEndDateIndex = message.IndexOf("AM");
        //    int pmEndDateIndex = message.IndexOf("PM");

        //    amEndDateIndex += 2; pmEndDateIndex += 2;

        //    string amPrefix = message.Remove(amEndDateIndex);
        //    string pmPrefix = message.Remove(pmEndDateIndex);

        //    DateTime dateTime;
        //    if (DateTime.TryParse(amPrefix, out dateTime)) return message.Substring(amEndDateIndex);
        //    if (DateTime.TryParse(pmPrefix, out dateTime)) return message.Substring(pmEndDateIndex);

        //    return message;
        //}

        /// <summary>
        /// log message based on log level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        private void Log(string message, LogEventArgs e)
        {
            switch (e.LogLevel)
            {
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;

                case LogLevel.Error:
                    _logger.Error(message);
                    break;

                case LogLevel.Fatal:
                    _logger.Fatal(message);
                    break;

                case LogLevel.Info:
                    _logger.Info(message);
                    break;

                case LogLevel.Warning:
                    _logger.Warn(message);
                    break;

                default:
                    throw new Exception("Unsupported log level.");
            }
        }

        /// <summary>
        /// log message based on log level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="e"></param>
        //private void Log(string message, Exception exception, LogEventArgs e)
        //{
        //    switch (e.LogLevel)
        //    {
        //        case LogLevel.Debug:
        //            _logger.Debug(message, exception);
        //            break;

        //        case LogLevel.Error:
        //            _logger.Error(message, exception);
        //            break;

        //        case LogLevel.Fatal:
        //            _logger.Fatal(message, exception);
        //            break;

        //        case LogLevel.Info:
        //            _logger.Info(message, exception);
        //            break;

        //        case LogLevel.Warning:
        //            _logger.Warn(message, exception);
        //            break;

        //        default:
        //            throw new Exception("Unsupported log level.");
        //    }

        //}
    }
}
