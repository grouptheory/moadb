using System;
using System.Collections.Generic;
using System.Text;

namespace logger
{
    public interface IObserver
    {
        string Name
        {
            get;
        }

        /// <summary>
        /// Register should only be called by the ILogger
        /// </summary>
        /// <param name="logger"></param>
        void Register(ILog logger);

        /// <summary>
        /// Unregister should only be called by the ILogger
        /// </summary>
        /// <param name="logger"></param>
        void Unregister(ILog logger);
		
		void SetThreshold(object key, LogLevel lvl);
		
		LogLevel DEFAULT_THRESHOLD {
			get;
			set;
		}
    }
}
