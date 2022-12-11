using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class AccountSplit : IAccountSplit
    {
        public AccountSplit(IAccount acc, string isReconciled, decimal trxVal)
        {
            Account = acc;
            IsReconciled = isReconciled;
            TrxValue = trxVal;
        }

        public IAccount Account { get; set; }
        public string IsReconciled { get; set; }
        public decimal TrxValue { get; set; }

        public override string ToString()
        {
            return $"{Account.ToString()} / {TrxValue.ToString()}";
        }
    }
}
