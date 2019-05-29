using System;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public interface ITransaction
    {
        string TransactionGuid { get; set; }
        IEnumerable<IAccount> Accounts{ get; set; }
        DateTime DatePosted { get; set; }
        int ChequeNumber { get; set; }
        string Description { get; set; }
        string Memo { get; set; }
        IEnumerable<IAccount> Categories { get; set; }
        string Reconciled { get; set; }
        decimal Value { get; set; }

        string QifOutput();
    }
}
