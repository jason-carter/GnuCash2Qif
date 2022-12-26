using System;
using System.Collections.Generic;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Transaction : ITransaction
    {
        public Transaction()
        {
            AccountSplits = new List<IAccountSplit>();
        }

        public Transaction(string transactionGuid, string accountGuid, string accountName, DateTime datePosted, string reference,
                           string description, string memo, string isReconciled, decimal trxValue, IDictionary<string, IAccount> accounts)
        {
            TransactionGuid = transactionGuid;
            AccountGuid = accountGuid;
            AccountName = accountName;
            DatePosted = datePosted;
            Reference = reference;
            Description = description;
            Memo = memo;
            AccountSplits = new List<IAccountSplit>();
            IsAccountSide = false;

            try
            {
                var account = accounts[accountGuid];

                // Wrap the account up into a split to represent double entry accounting
                AccountSplits.Add(new AccountSplit(account, isReconciled, trxValue));

                if (account.IsAccount)
                {
                    IsAccountSide = true;
                    account.Transactions.Add(transactionGuid, this);
                }
            }
            catch (KeyNotFoundException knfe)
            {
                throw new UnknownAccountException(accountGuid);

            }
        }

        public string TransactionGuid { get; set; }
        public string AccountGuid { get; set; }
        public string AccountName { get; set; }
        public DateTime DatePosted { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Memo { get; set; }
        public bool IsAccountSide { get; }
        public List<IAccountSplit> AccountSplits { get; set; }

        public override string ToString()
        {
            return $"{DatePosted.ToString("yyyy-MM-dd")} / {Description} / {Memo}";
        }
    }
}
