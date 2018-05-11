using System;
using System.Collections.Generic;
using System.Text;

namespace logger
{
    public class LogEventArgs:EventArgs
    {
        private object _sender;

        private string _message;
        private Exception _ex;
        private string _key;
        private LogLevel _logLevel;


        public LogEventArgs(object sender, string message, Exception ex, string key, LogLevel logLevel)
        {
            _sender = sender;
            _message = message;
            _ex = ex;
            _key = key;
            _logLevel = logLevel;
        }


        public object Sender
        {
            get { return _sender; }
        }

        public string Message
        {
            get { return _message; }
        }

        public Exception Exception
        {
            get { return _ex; }
        }

        public string Key
        {
            get { return _key; }
        }

        public LogLevel LogLevel
        {
            get { return _logLevel; }
        }

    }
}
