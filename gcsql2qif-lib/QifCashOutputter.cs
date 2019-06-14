using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GnuCash.Sql2Qif.Library.BLL;
using System.Linq;

namespace GnuCash.Sql2Qif.Library
{
    class QifCashOutputter : ICashOutputter
    {
        public void Write(List<IAccount> accounts, string outputFileName)
        {
            using (var writer = File.CreateText(outputFileName))
            {
                Write(accounts, writer);
            }
        }

        public void Write(List<IAccount> accounts, StreamWriter outputFile)
        {
            // Category section (expense / income accounts)
            outputFile.WriteLine("!Type:Cat");
            accounts.FindAll(n => IsCategory(n.AccountType))
                .ToList().ForEach(n => QifCategoryOutput(n, outputFile));

            outputFile.WriteLine("!Option:AutoSwitch"); // TODO: Check what this does6

            // Account section (asset / credit / bank / liability accounts)
            outputFile.WriteLine("!Account");
            accounts.FindAll(n => IsAccount(n.AccountType))
                .ToList().ForEach(n => QifAccountOutput(n, outputFile));

            outputFile.WriteLine("!Clear:AutoSwitch");  // TODO: Check what this does6
            outputFile.WriteLine("!Option:AutoSwitch"); // TODO: Check what this does6

            // Transaction section by account
            accounts.FindAll(n => IsAccount(n.AccountType))
                .ToList().ForEach(n => QifAccountTransactionOutput(n, outputFile));
        }

        private void QifAccountTransactionOutput(IAccount acc, StreamWriter output)
        {
            QifAccountTransactionHeaderOutput(acc, output);
            acc.Transactions.ForEach(t => QifTransactionOutput(t, output));
        }

        private void QifTransactionOutput(ITransaction trx, StreamWriter output)
        {
            output.WriteLine($"D{trx.DatePosted.ToString("MM/d/yyyy")}"); // TODO: Check QIF's supported date formats
            output.WriteLine($"U{trx.Value}");
            output.WriteLine($"T{trx.Value}");
            output.WriteLine($"P{trx.Memo}");
            if (trx.Reconciled.ToLower().Equals("y") ||
                trx.Reconciled.ToLower().Equals("c"))
            {
                // Reconciled or Cleared
                output.WriteLine($"C*");
            }
            if (trx.Categories.Count<IAccount>() > 0)
            {
                //TODO: Category or account transfer
                // Account name is enclosed in square brackets
                output.WriteLine($"L{(trx.Categories.FirstOrDefault<IAccount>()).Name}");
            }
            output.WriteLine($"^");
        }

        private void QifAccountTransactionHeaderOutput(IAccount acc, StreamWriter output)
        {
            output.WriteLine($"!Account");
            output.WriteLine($"N{acc.Name}");
            output.WriteLine($"T{QifAccountType(acc.AccountType)}");
            output.WriteLine($"^");
            output.WriteLine($"!Type:{QifAccountType(acc.AccountType)}");
        }

        private void QifAccountOutput(IAccount acc, StreamWriter output)
        {
            output.WriteLine($"N{acc.Name}");
            output.WriteLine($"T{QifAccountType(acc.AccountType)}");
            output.WriteLine($"D{acc.Description}");
            output.WriteLine($"^");
        }

        private void QifCategoryOutput(IAccount cat, StreamWriter output)
        {
            output.WriteLine($"N{cat.Name}");
            output.WriteLine($"D{cat.Description}");
            output.WriteLine($"{QifCategoryType(cat.AccountType)}");
            output.WriteLine($"^");
        }

        private bool IsCategory(string catType)
        {
            return (catType == "EXPENSE" || 
                    catType == "INCOME") ? true : false;
        }

        private bool IsAccount(string catType)
        {
            return (catType == "ASSET" ||
                    catType == "CREDIT" ||
                    catType == "BANK" ||
                    catType == "LIABILITY") ? true : false;
        }

        private string QifCategoryType(string catType)
        {
            return catType == "INCOME" ? "I" :
                   catType == "EXPENSE" ? "E" :
                   "?";
        }

        private string QifAccountType(string accType)
        {
            return accType == "BANK" ? "Bank" :
                    accType == "CREDIT" ? "CCard" :
                    accType == "ASSET" ? "Oth A" :
                    accType == "LIABILITY" ? "Oth L" :
                    "?";
        }
    }
}
