using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Account : IAccount
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public string Hierarchy { get; set; }
        public int HierarchyLevel { get; set; }
        public IEnumerable<ITransaction> Transactions { get; set; }

        public string QifOutput()
        {
            var qif = $"N{Name}{Environment.NewLine}";
            qif += $"T{QifAccountType()}{Environment.NewLine}";
            qif += $"D{Description}{Environment.NewLine}";
            qif += $"^{Environment.NewLine}";

            return qif;
        }

        public string QifAccountTransactionHeaderOutput()
        {
            var qif = $"!Account{Environment.NewLine}";
            qif += $"N{Name}{Environment.NewLine}";
            qif += $"T{QifAccountType()}{Environment.NewLine}";
            qif += $"^{Environment.NewLine}";
            qif += $"!Type:{QifAccountType()}{Environment.NewLine}";

            return qif;
        }

        private string QifAccountType()
        {
            return  AccountType == "BANK" ? "Bank" : 
                    AccountType == "CREDIT" ? "CCard" : 
                    AccountType == "ASSET" ? "Oth A" : 
                    "?";
        }
    }
}
