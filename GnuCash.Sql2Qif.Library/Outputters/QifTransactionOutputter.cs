using GnuCash.Sql2Qif.Library.DTO;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.Outputters
{
    internal class QifTransactionOutputter : QifOutputterBase<IAccount>
    {
        public QifTransactionOutputter(ILogger<IAccount> logger, StreamWriter writer) : base(logger, writer) { }

        public override void Write(IDictionary<string, IAccount> accounts)
        {
            writer.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list (with transactions this time)

            // Transaction section by account
            accounts.Values.Where(n => n.IsAccount).ToList().ForEach(acc => QifAccountTransactionOutput(acc));
        }

        private void QifAccountTransactionOutput(IAccount acc)
        {
            QifAccountTransactionHeaderOutput(acc);
            acc.Transactions.ForEach(t => QifTransactionOutput(acc, t));
        }

        private void QifTransactionOutput(IAccount parentAcc, ITransaction trx)
        {
            writer.WriteLine($"D{trx.DatePosted.ToString("MM/d/yyyy")}"); // TODO: Check QIF's supported date formats
            if (trx.Reference != null && !trx.Reference.Equals(string.Empty))
            {
                writer.WriteLine($"N{trx.Reference}");
            }
            writer.WriteLine($"U{trx.TrxValue}");
            writer.WriteLine($"T{trx.TrxValue}");
            writer.WriteLine($"P{trx.Description}");
            writer.WriteLine($"M{trx.Memo}");
            if (IsReconciled(trx.IsReconciled))
            {
                writer.WriteLine($"C*");
            }
            writer.WriteLine($"L{trx.AccountReference}");
            writer.WriteLine($"^");
        }

        private void QifAccountTransactionHeaderOutput(IAccount acc)
        {
            writer.WriteLine($"!Account");
            writer.WriteLine($"N{acc.Name}");
            writer.WriteLine($"T{QifAccountType(acc.AccountType)}");
            writer.WriteLine($"^");
            writer.WriteLine($"!Type:{QifAccountType(acc.AccountType)}");
        }
    }
}
