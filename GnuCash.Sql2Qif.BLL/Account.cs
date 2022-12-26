using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Account : IAccount
    {
        public Account()
        {
            Transactions = new Dictionary<string, ITransaction>();
        }

        public Account(string guid, string name, string description, string accountType)
        {
            Guid = guid;
            Name = name;
            Description = description;
            AccountType = accountType;
            Transactions = new Dictionary<string, ITransaction>();
        }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public Dictionary<string, ITransaction> Transactions { get; set; }

        public bool IsAccount => AccountType.ToUpper().Equals("ASSET") ||
                                 AccountType.ToUpper().Equals("CREDIT") ||
                                 AccountType.ToUpper().Equals("BANK");

        public bool IsCategory => AccountType.ToUpper().Equals("EXPENSE") ||
                                  AccountType.ToUpper().Equals("INCOME");

        public override string ToString()
        {
            return $"{AccountType} / {Name}";
        }
    }
}
