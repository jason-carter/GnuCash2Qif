using GnuCash.Sql2Qif.Library.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.Outputters
{
    internal class QifTransactionOutputter : QifOutputterBase<IAccount>
    {
        public QifTransactionOutputter(IProgress<string> progress, StreamWriter writer) : base(progress, writer) { }

        public override void Write(IDictionary<string, IAccount> accounts)
        {
            Writer.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list (with transactions this time)

            // Transaction section by account
            accounts.Values.Where(n => n.IsAccount).ToList().ForEach(QifAccountTransactionOutput);
        }

        private void QifAccountTransactionOutput(IAccount acc)
        {
            QifAccountTransactionHeaderOutput(acc);
            acc.Transactions.ForEach(t => QifTransactionOutput(acc, t));
        }

        private void QifTransactionOutput(IAccount parentAcc, ITransaction trx)
        {
            Writer.WriteLine($"D{trx.DatePosted.ToString("MM/d/yyyy")}"); // TODO: Check QIF's supported date formats
            if (trx.Reference != null && !trx.Reference.Equals(string.Empty))
            {
                Writer.WriteLine($"N{trx.Reference}");
            }
            Writer.WriteLine($"U{trx.TrxValue}");
            Writer.WriteLine($"T{trx.TrxValue}");
            Writer.WriteLine($"P{trx.Description}");
            Writer.WriteLine($"M{trx.Memo}");
            if (IsReconciled(trx.IsReconciled))
            {
                Writer.WriteLine($"C*");
            }
            Writer.WriteLine($"L{trx.AccountReference}");
            Writer.WriteLine($"^");
        }

        private void QifAccountTransactionHeaderOutput(IAccount acc)
        {
            Writer.WriteLine($"!Account");
            Writer.WriteLine($"N{acc.Name}");
            Writer.WriteLine($"T{QifAccountType(acc.AccountType)}");
            Writer.WriteLine($"^");
            Writer.WriteLine($"!Type:{QifAccountType(acc.AccountType)}");
        }
    }
}
