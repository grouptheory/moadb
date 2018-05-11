using System;
using System.Collections.Generic;
using System.Text;

namespace logger
{

    public interface ILog
    {
		void SilentLogEvents (bool silentLogEvents);
		
    	//Logging methods
        void InfoLog(object sender, string message);
        void InfoLog(object sender, string message, string key);
        void InfoLog(object sender, string message, Exception ex);
        void InfoLog(object sender, string message, Exception ex, string key);

        void DebugLog(object sender, string message);
        void DebugLog(object sender, string message, string key);
        void DebugLog(object sender, string message, Exception ex);
        void DebugLog(object sender, string message, Exception ex, string key);

        void WarningLog(object sender, string message);
        void WarningLog(object sender, string message, string key);
        void WarningLog(object sender, string message, Exception ex);
        void WarningLog(object sender, string message, Exception ex, string key);

        void ErrorLog(object sender, string message);
        void ErrorLog(object sender, string message, string key);
        void ErrorLog(object sender, string message, Exception ex);
        void ErrorLog(object sender, string message, Exception ex, string key);

        void FatalLog(object sender, string message);
        void FatalLog(object sender, string message, string key);
        void FatalLog(object sender, string message, Exception ex);
        void FatalLog(object sender, string message, Exception ex, string key);

        event EventHandler<LogEventArgs> LogRequest;

        void Attach(IObserver observer);
        void Detach(IObserver observer);

    }
}
