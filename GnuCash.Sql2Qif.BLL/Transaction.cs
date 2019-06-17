using System;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Transaction : ITransaction
    {
        public Transaction()
        {
            AccountSplits = new List<IAccountSplit>();
            ParentAccounts = new List<IAccount>();
        }

        public string TransactionGuid { get; set; }
        public DateTime DatePosted { get; set; }
        public string Ref { get; set; }
        public string Description { get; set; }
        public string Memo { get; set; }
        public decimal Value { get; set; }
        public List<IAccountSplit> AccountSplits { get; set; }
        public List<IAccount> ParentAccounts { get; set; }
    }
}
