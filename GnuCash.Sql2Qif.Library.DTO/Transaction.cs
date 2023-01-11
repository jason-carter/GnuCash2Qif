using System;

namespace GnuCash.Sql2Qif.Library.DTO
{
    public class Transaction : ITransaction
    {
        public Transaction(string transactionGuid, string accountGuid, string accountName, DateTime datePosted, string reference,
                   string description, string memo, string isReconciled, decimal trxValue)
        {
            TransactionGuid = transactionGuid;
            AccountGuid = accountGuid;
            AccountName = accountName;
            DatePosted = datePosted;
            Reference = reference;
            Description = description;
            Memo = memo;
            IsAccountSide = false;
            IsReconciled = isReconciled;
            TrxValue = trxValue;
        }

        public string TransactionGuid { get; set; }
        public string AccountGuid { get; set; }
        public string AccountName { get; set; }
        public DateTime DatePosted { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Memo { get; set; }
        public bool IsAccountSide { get; }
        public string IsReconciled { get; set; }
        public decimal TrxValue { get; set; }
        public string AccountReference { get; set; }

        public override string ToString()
        {
            return $"{DatePosted:yyyy-MM-dd} / {Description} / {Memo}";
        }
    }
}
