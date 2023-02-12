using System;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.DTO
{
    public interface ITransaction
    {
        string TransactionGuid { get; set; }
        string AccountGuid {  get; set; }
        DateTime DatePosted { get; set; }
        string Reference { get; set; }
        string Description { get; set; }
        string Memo { get; set; }
        string IsReconciled {  get; set; }
        decimal TrxValue { get; set; }
        string AccountReference { get; set; }
    }
}
