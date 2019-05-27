using System;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Category : IAccount
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
            var nl = Environment.NewLine;
            var qif = $"N{Name}{nl}";
            qif += $"D{Description}{nl}";
            qif += $"{QifAccountType()}{nl}";
            qif += $"^{Environment.NewLine}";
            return qif;
        }

        private string QifAccountType()
        {
            return AccountType == "INCOME" ? "I" : 
                   AccountType == "EXPENSE" ? "E" : 
                   "?";
        }
    }
}
