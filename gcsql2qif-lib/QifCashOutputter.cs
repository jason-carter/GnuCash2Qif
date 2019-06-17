using GnuCash.Sql2Qif.Library.BLL;
using System.Collections.Generic;
using System.IO;
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
            WriteCategoryList(accounts, outputFile);
            WriteAccountList(accounts, outputFile);
            WriteTransactionListByAccount(accounts, outputFile);
        }

        private void WriteTransactionListByAccount(List<IAccount> accounts, StreamWriter outputFile)
        {
            outputFile.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list (with transactions this time)

            // Transaction section by account
            accounts.FindAll(n => IsAccount(n.AccountType))
                .ToList().ForEach(n => QifAccountTransactionOutput(n, outputFile));
        }

        private void QifAccountTransactionOutput(IAccount acc, StreamWriter output)
        {
            QifAccountTransactionHeaderOutput(acc, output);
            acc.Transactions.ForEach(t => QifTransactionOutput(acc, t, output));
        }

        private void QifTransactionOutput(IAccount parentAcc, ITransaction trx, StreamWriter output)
        {
            // Use the transaction value of the parent account
            var trxValue = trx.AccountSplits.Where(ac => IsAccount(ac.Account.AccountType) && ac.Account.Guid == parentAcc.Guid).FirstOrDefault().Trxvalue;
            var accountRef = "";

            if (trx.AccountSplits.Where(ac => IsCategory(ac.Account.AccountType)).Count<IAccountSplit>() > 0)
            {
                // Has one or more categories
                accountRef += $"{trx.AccountSplits.Where(ac => IsCategory(ac.Account.AccountType)).FirstOrDefault().Account.Name}";
            }
            else if (trx.AccountSplits.Where(ac => IsAccount(ac.Account.AccountType)).Count<IAccountSplit>() > 1)
            {
                // Has more than one account so is an account transfer
                //TODO: Check if there could be multiple account transfers in a split - tricky
                accountRef += $"[{trx.AccountSplits.Where(ac => IsAccount(ac.Account.AccountType) && ac.Account.Guid != parentAcc.Guid).FirstOrDefault().Account.Name}]";
            }
            else
            {
                //TODO: Warning Transaction with no categories and only one account reference
            }

            output.WriteLine($"D{trx.DatePosted.ToString("MM/d/yyyy")}"); // TODO: Check QIF's supported date formats
            output.WriteLine($"U{trxValue}");
            output.WriteLine($"T{trxValue}");
            output.WriteLine($"P{trx.Description}");
            output.WriteLine($"M{trx.Memo}");

            if (trx.Reconciled.ToLower().Equals("y") ||
                trx.Reconciled.ToLower().Equals("c"))
            {
                // Reconciled or Cleared
                output.WriteLine($"C*");
            }
            output.WriteLine($"L{accountRef}");
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

        private void WriteAccountList(List<IAccount> accounts, StreamWriter outputFile)
        {
            // Account section (asset / credit / bank / liability accounts)

            outputFile.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list

            outputFile.WriteLine("!Account");
            accounts.FindAll(n => IsAccount(n.AccountType))
                .ToList().ForEach(n => QifAccountOutput(n, outputFile));

            outputFile.WriteLine("!Clear:AutoSwitch");  // Indicates end of the account list
        }

        private void QifAccountOutput(IAccount acc, StreamWriter output)
        {
            output.WriteLine($"N{acc.Name}");
            output.WriteLine($"T{QifAccountType(acc.AccountType)}");
            output.WriteLine($"D{acc.Description}");
            output.WriteLine($"^");
        }

        private void WriteCategoryList(List<IAccount> accounts, StreamWriter outputFile)
        {
            // Category section (expense / income accounts)
            outputFile.WriteLine("!Type:Cat");
            accounts.FindAll(n => IsCategory(n.AccountType))
                .ToList().ForEach(n => QifCategoryOutput(n, outputFile));
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
                   "?"; // TODO: WARNING Unknown Category Type
        }

        private string QifAccountType(string accType)
        {
            return accType == "BANK" ? "Bank" :
                    accType == "CREDIT" ? "CCard" :
                    accType == "ASSET" ? "Oth A" :
                    accType == "LIABILITY" ? "Oth L" :
                    "?"; // TODO: WARNING Unknown Account Type
        }
    }
}
