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
            Writer.WriteLine("!Type:Cat");
            accounts.Values.Where(n => n.IsCategory).ToList().ForEach(QifCategoryOutput);
        }

        private void QifCategoryOutput(IAccount cat)
        {
            Writer.WriteLine($"N{cat.Name}");
            Writer.WriteLine($"D{cat.Description}");
            Writer.WriteLine($"{QifCategoryType(cat.AccountType)}");
            Writer.WriteLine($"^");
        }
    }
}
