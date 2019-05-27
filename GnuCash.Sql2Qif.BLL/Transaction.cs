using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Transaction : ITransaction
    {
        public string TransactionGuid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime DatePosted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ChequeNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Memo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IEnumerable<IAccount> Categories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Reconciled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public decimal Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string QifOutput()
        {
            throw new NotImplementedException();
        }
    }
}
