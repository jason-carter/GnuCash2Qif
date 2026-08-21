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
            Writer.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list

            Writer.WriteLine("!Account");
            accounts.Values.Where(n => n.IsAccount).ToList().ForEach(QifAccountOutput);

            Writer.WriteLine("!Clear:AutoSwitch");  // Indicates end of the account list
        }

        private void QifAccountOutput(IAccount acc)
        {
            Writer.WriteLine($"N{acc.Name}");
            Writer.WriteLine($"T{QifAccountType(acc.AccountType)}");
            Writer.WriteLine($"D{acc.Description}");
            Writer.WriteLine($"^");
        }
    }
}
