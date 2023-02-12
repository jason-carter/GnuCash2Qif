using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.DTO
{
    public class Account : IAccount
    {
        public Account(string guid, string name, string description, string accountType)
        {
            Guid = guid;
            Name = name;
            Description = description;
            AccountType = accountType;
            Transactions = new List<ITransaction>();
        }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public List<ITransaction> Transactions { get; set; }

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
