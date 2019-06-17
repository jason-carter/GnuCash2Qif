using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class AccountSplit : IAccountSplit
    {
        public IAccount Account { get; set; }
        public decimal Trxvalue { get; set; }
    }
}
