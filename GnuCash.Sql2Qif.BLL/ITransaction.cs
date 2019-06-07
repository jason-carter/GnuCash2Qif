using System;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public interface ITransaction
    {
        string TransactionGuid { get; set; }
        DateTime DatePosted { get; set; }
        string Ref { get; set; }
        string Description { get; set; }
        string Memo { get; set; }
        List<IAccount> Categories { get; set; }
        string Reconciled { get; set; }
        decimal Value { get; set; }

        string QifOutput();
    }
}
