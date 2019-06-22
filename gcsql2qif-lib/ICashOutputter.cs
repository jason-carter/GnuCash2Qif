using System.Collections.Generic;
using System.IO;
using GnuCash.Sql2Qif.Library.BLL;

namespace GnuCash.Sql2Qif.Library
{
    interface ICashOutputter
    {
        void Write(List<IAccount> accounts, string outputFileName);
    }
}
