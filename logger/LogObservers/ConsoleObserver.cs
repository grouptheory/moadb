using System;
using System.Collections.Generic;
using System.Text;

namespace logger
{
    public class ConsoleObserver : AbstractObserver
    {

        public ConsoleObserver() : this("") { }

        public ConsoleObserver(string key) : base(key) { }


        public override string Name
        {
            get { return "Console Observer"; }
        }


        protected override void logger_LogRequest(object sender, LogEventArgs e)
        {
            string message = ":> " + e.Message;

            Console.WriteLine(message);

            if (e.Exception != null)
            {
                Console.WriteLine(e.Exception);
            }

        }


    }
}