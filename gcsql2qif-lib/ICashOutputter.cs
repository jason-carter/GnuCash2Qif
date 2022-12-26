using System.Collections.Generic;
using GnuCash.Sql2Qif.Library.BLL;

namespace GnuCash.Sql2Qif.Library
{
    public interface ICashOutputter
    {
        void Write(IDictionary<string, IAccount> accounts, string outputFileName);
    }
}
