using GnuCash.Sql2Qif.Library.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.Outputters
{
    internal class QifCategoryOutputter : QifOutputterBase<IAccount>
    {
        public QifCategoryOutputter(IProgress<string> progress, StreamWriter writer) : base(progress, writer) { }

        public override void Write(IDictionary<string, IAccount> accounts)
        {
            // Category section (expense / income accounts)
            writer.WriteLine("!Type:Cat");
            accounts.Values.Where(n => n.IsCategory).ToList().ForEach(cat => QifCategoryOutput(cat));
        }

        private void QifCategoryOutput(IAccount cat)
        {
            writer.WriteLine($"N{cat.Name}");
            writer.WriteLine($"D{cat.Description}");
            writer.WriteLine($"{QifCategoryType(cat.AccountType)}");
            writer.WriteLine($"^");
        }
    }
}
