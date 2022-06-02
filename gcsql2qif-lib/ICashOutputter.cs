using System;
using System.Collections.Generic;
using System.IO;
using GnuCash.Sql2Qif.Library.BLL;

namespace GnuCash.Sql2Qif.Library
{
    public interface ICashOutputter
    {
        event EventHandler<LogEventArgs> LogEvent;
        void Write(List<IAccount> accounts, string outputFileName);
    }
}
