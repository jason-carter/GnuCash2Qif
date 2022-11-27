using System.Collections.Generic;
using GnuCash.Sql2Qif.Library.BLL;

namespace GnuCash.Sql2Qif.Library
{
    public interface ICashOutputter
    {
        void Write(List<IAccount> accounts, string outputFileName);
    }
}
