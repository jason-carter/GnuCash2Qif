using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Transaction : ITransaction
    {
        public string TransactionGuid { get; set; }
        public IEnumerable<IAccount> Accounts { get; set; }
        public DateTime DatePosted { get; set; }
        public int ChequeNumber { get; set; }
        public string Description { get; set; }
        public string Memo { get; set; }
        public IEnumerable<IAccount> Categories { get; set; }
        public string Reconciled { get; set; }
        public decimal Value { get; set; }

        public string QifOutput()
        {
            return "";
        }
    }
}
