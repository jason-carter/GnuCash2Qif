using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public interface IAccount
    {
        string Guid { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string AccountType { get; set; }
        bool IsAccount { get; }
        bool IsCategory { get; }
        Dictionary<string, ITransaction> Transactions { get; set; }
    }
}
