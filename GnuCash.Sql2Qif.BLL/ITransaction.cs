using System;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public interface ITransaction
    {
        string TransactionGuid { get; set; }
        DateTime DatePosted { get; set; }
        string Reference { get; set; }
        string Description { get; set; }
        string Memo { get; set; }
        bool IsAccountSide { get; }
        List<IAccountSplit> AccountSplits { get; set; }
        List<IAccount> ParentAccounts { get; set; }
    }
}
