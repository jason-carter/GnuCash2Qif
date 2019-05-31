using System;

namespace GnuCash.Sql2Qif.Library
{
    public class LogEventArgs : EventArgs
    {
        public string LogLevel { get; set; }
        public string LogMessage { get; set; }
    }
}
