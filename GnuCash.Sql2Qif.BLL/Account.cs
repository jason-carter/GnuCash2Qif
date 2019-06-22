using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Account : IAccount
    {
        public Account ()
        {
            Transactions = new List<ITransaction>();
        }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public string Hierarchy { get; set; }
        public int HierarchyLevel { get; set; }
        public List<ITransaction> Transactions { get; set; }

        public override string ToString()
        {
            return $"{AccountType} / {Name}";
        }
    }
}
