using GnuCash.Sql2Qif.Library.BLL;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.DAL
{
    public interface ITransactionDAO
    {
        IDictionary<string, ITransaction> Extract(string dataSource, IDictionary<string, IAccount> accounts);
    }
}
