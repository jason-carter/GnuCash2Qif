using GnuCash.Sql2Qif.Library.BLL;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.DAL
{
    public interface ITransactionDAO
    {
        IEnumerable<ITransaction> Extract(string dataSource, IEnumerable<IAccount> accounts);
    }
}
