using System;
using System.IO;

namespace logger
{
	public class LoggerInitialization
	{
		private static Log4NetObserver _log4net;
		private static Boolean _setupCompleted = false;
		
		public static void Setup() {
			if (_setupCompleted) return;
			_setupCompleted = true;
			
			String configfile = "logger.config.xml";
			ILog log = SingletonLogger.Instance();

			_log4net = new Log4NetObserver(configfile);
			log.Attach(_log4net);

			// Console.WriteLine("GGG Log4NetObserver: ready");
		}
		
		public void SilentLogEvents (bool silentLogEvents) {
			if (!_setupCompleted) {
				Setup();
			}
			SingletonLogger.Instance().SilentLogEvents (silentLogEvents);
		}
		
		public static void SetThreshold(object sender, LogLevel lvl) {
			if (!_setupCompleted) {
				Setup();
			}

			_log4net.SetThreshold(sender, lvl);

			// Console.WriteLine("GGG Log4NetObserver: setting threshold of "+sender+" to "+lvl);
		}
		
		public LogLevel DEFAULT_THRESHOLD {
			get { return _log4net.DEFAULT_THRESHOLD; }
			set { _log4net.DEFAULT_THRESHOLD = value; }
		}
	}
}

