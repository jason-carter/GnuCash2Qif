using GnuCash.Sql2Qif.Library.BLL;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.DAL
{
    public interface IAccountDAO
    {
        IDictionary<string, IAccount> Extract(string dataSource);
    }
}
