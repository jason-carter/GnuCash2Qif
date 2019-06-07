using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Transaction : ITransaction
    {
        public Transaction()
        {
            Categories = new List<IAccount>();
        }

        public string TransactionGuid { get; set; }
        public DateTime DatePosted { get; set; }
        public string Ref { get; set; }
        public string Description { get; set; }
        public string Memo { get; set; }
        public List<IAccount> Categories { get; set; }
        public string Reconciled { get; set; }
        public decimal Value { get; set; }

        public string QifOutput()
        {
            var qif = $"D{DatePosted.ToString("MM/d/yyyy")}{Environment.NewLine}"; // TODO: Check QIF's supported date formats
            qif += $"U{Value}{Environment.NewLine}";
            qif += $"T{Value}{Environment.NewLine}";
            qif += $"P{Memo}{Environment.NewLine}";
            if (Reconciled.ToLower().Equals("y") || 
                Reconciled.ToLower().Equals("c"))
            {
                // Reconciled or Cleared
                qif += $"C*{Environment.NewLine}";
            }
            if (Categories.Count<IAccount>() > 0)
            {
                qif += $"L{(Categories.FirstOrDefault<IAccount>()).Name}{Environment.NewLine}";
            }
            qif += $"^{Environment.NewLine}";

            return qif;
        }
    }
}
