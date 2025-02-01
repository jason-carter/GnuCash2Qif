using GnuCash.Sql2Qif.Library.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.Outputters
{
    internal class QifAccountOutputter : QifOutputterBase<IAccount>
    {
        public QifAccountOutputter(IProgress<string> progress, StreamWriter writer) : base(progress, writer) { }

        public override void Write(IDictionary<string, IAccount> accounts)
        {
            // Account section (asset / credit / bank / liability accounts)
            writer.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list

            writer.WriteLine("!Account");
            accounts.Values.Where(n => n.IsAccount).ToList().ForEach(acc => QifAccountOutput(acc));

            writer.WriteLine("!Clear:AutoSwitch");  // Indicates end of the account list
        }

        private void QifAccountOutput(IAccount acc)
        {
            writer.WriteLine($"N{acc.Name}");
            writer.WriteLine($"T{QifAccountType(acc.AccountType)}");
            writer.WriteLine($"D{acc.Description}");
            writer.WriteLine($"^");
        }
    }
}
