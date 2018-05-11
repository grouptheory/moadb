using System;
using System.Collections.Generic;
using System.Text;

namespace logger
{
    /// <summary>
    /// Singleton implementation of the ILogger logging interface
    /// </summary>
    public sealed class SingletonLogger : ILog
    {
        /// <summary>
        /// Private instance of the logger cannot be accessed directly outside of the class.
        /// Readonly such that the logger can only be set once.
        /// </summary>
        private static readonly SingletonLogger _logger = new SingletonLogger();

        //private Dictionary<string, IObserver> _observers;
        private List<IObserver> _observerList;
        /// <summary>
        /// Returns an instance of the Logger.
        /// </summary>
        public static SingletonLogger Instance()
        {
			LoggerInitialization.Setup();
            return _logger;
        }

        private SingletonLogger()
        {
            //_observers = new Dictionary<string, IObserver>();
            _observerList = new List<IObserver>();
        }

        public event EventHandler<LogEventArgs> LogRequest;

		
		private bool _silentLogEvents;
		public void SilentLogEvents (bool silentLogEvents)
		{
			_silentLogEvents = silentLogEvents;
		}


        private void RaiseEvent (object sender, string message, Exception ex, string key, LogLevel logLevel)
		{
			if (_silentLogEvents)
				return;

			// Console.WriteLine("GGG RaiseEvent: "+message);

            if (LogRequest != null)
            {
                LogEventArgs e = new LogEventArgs(sender, message, ex, key, logLevel);
                LogRequest(this, e);
            }
        }

        #region ILogger Registration Methods

        public void Attach(IObserver observer)
        {
            //_observers.Add(observer.Name, observer);
            _observerList.Add(observer);
            observer.Register(this);
        }

        public void Detach(IObserver observer)
        {
            //if (_observers.ContainsKey(observer.Name))
            //{
            //    _observers.Remove(observer.Name);
            //    observer.Unregister(this);
            //}

            if (_observerList.Contains(observer))
            {
                _observerList.Remove(observer);
                observer.Unregister(this);
            }
        }

        #endregion

        #region ILog Members

        public void InfoLog(object sender, string message)
        {
            RaiseEvent(sender, message, null, null, LogLevel.Info);
        }

        public void InfoLog(object sender, string message, string key)
        {
            RaiseEvent(sender, message, null, key, LogLevel.Info);
        }

        public void InfoLog(object sender, string message, Exception ex)
        {
            RaiseEvent(sender, message, ex, null, LogLevel.Info);
        }

        public void InfoLog(object sender, string message, Exception ex, string key)
        {
            RaiseEvent(sender, message, ex, key, LogLevel.Info);
        }

        public void DebugLog(object sender, string message)
        {
            RaiseEvent(sender, message, null, null, LogLevel.Debug);
        }

        public void DebugLog(object sender, string message, string key)
        {
            RaiseEvent(sender, message, null, key, LogLevel.Debug);
        }

        public void DebugLog(object sender, string message, Exception ex)
        {
            RaiseEvent(sender, message, ex, null, LogLevel.Debug);
        }

        public void DebugLog(object sender, string message, Exception ex, string key)
        {
            RaiseEvent(sender, message, ex, key, LogLevel.Debug);
        }

        public void WarningLog(object sender, string message)
        {
            RaiseEvent(sender, message, null, null, LogLevel.Warning);
        }

        public void WarningLog(object sender, string message, string key)
        {
            RaiseEvent(sender, message, null, key, LogLevel.Warning);
        }

        public void WarningLog(object sender, string message, Exception ex)
        {
            RaiseEvent(sender, message, ex, null, LogLevel.Warning);
        }

        public void WarningLog(object sender, string message, Exception ex, string key)
        {
            RaiseEvent(sender, message, ex, key, LogLevel.Warning);
        }

        public void ErrorLog(object sender, string message)
        {
            RaiseEvent(sender, message, null, null, LogLevel.Error);
        }

        public void ErrorLog(object sender, string message, string key)
        {
            RaiseEvent(sender, message, null, key, LogLevel.Error);
        }

        public void ErrorLog(object sender, string message, Exception ex)
        {
            RaiseEvent(sender, message, ex, null, LogLevel.Error);
        }

        public void ErrorLog(object sender, string message, Exception ex, string key)
        {
            RaiseEvent(sender, message, ex, key, LogLevel.Error);
        }

        public void FatalLog(object sender, string message)
        {
            RaiseEvent(sender, message, null, null, LogLevel.Fatal);
        }

        public void FatalLog(object sender, string message, string key)
        {
            RaiseEvent(sender, message, null, key, LogLevel.Fatal);
        }

        public void FatalLog(object sender, string message, Exception ex)
        {
            RaiseEvent(sender, message, ex, null, LogLevel.Fatal);
        }

        public void FatalLog(object sender, string message, Exception ex, string key)
        {
            RaiseEvent(sender, message, ex, key, LogLevel.Fatal);
        }
        #endregion
    }
}
