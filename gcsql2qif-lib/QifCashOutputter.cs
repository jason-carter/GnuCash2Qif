using GnuCash.Sql2Qif.Library.BLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library
{
    class QifCashOutputter : ICashOutputter
    {
        public event EventHandler<LogEventArgs> LogEvent;

        private void OnLogEvent(string level, string logMessage)
        {
            LogEventArgs args = new LogEventArgs()
            {
                LogLevel = level,
                LogMessage = logMessage
            };
            LogEvent?.Invoke(this, args);
        }

        public void Write(List<IAccount> accounts, string outputFileName)
        {
            using (var writer = File.CreateText(outputFileName))
            {
                OnLogEvent("INFO", "Writing category section...");
                WriteCategoryList(accounts, writer);
                OnLogEvent("INFO", "Writing accounts section...");
                WriteAccountList(accounts, writer);
                OnLogEvent("INFO", "Writing transactions by accounts...");
                WriteTransactionListByAccount(accounts, writer);
            }
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
            var mainParentAccount = trx.AccountSplits.Where(ac => IsAccount(ac.Account.AccountType) && ac.Account.Guid == parentAcc.Guid).FirstOrDefault();
            var trxValue = mainParentAccount.Trxvalue;
            var isReconciled = IsReconciled(mainParentAccount.Reconciled);
            var accountRef = "";

            if (trx.AccountSplits.Where(ac => IsCategory(ac.Account.AccountType)).Count<IAccountSplit>() > 0)
            {
                // Has one or more category accounts
                // TODO: Consider supporting multiple splits, for now just grab the first one
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
                OnLogEvent("WARNING", $"Transaction {trx.ToString()} has no categories and only one account reference ({mainParentAccount.ToString()})");
            }

            output.WriteLine($"D{trx.DatePosted.ToString("MM/d/yyyy")}"); // TODO: Check QIF's supported date formats
            if (trx.Ref != null && !trx.Ref.Equals(string.Empty))
            {
                output.WriteLine($"N{trx.Ref}");
            }
            output.WriteLine($"U{trxValue}");
            output.WriteLine($"T{trxValue}");
            output.WriteLine($"P{trx.Description}");
            output.WriteLine($"M{trx.Memo}");
            if (isReconciled)
            {
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

        private bool IsReconciled(string isReconciled)
        {
            // Assuming 'cleared' accounts are reconciled
            return (isReconciled.ToLower().Equals("y") ||
                    isReconciled.ToLower().Equals("c"));
        }

        private bool IsCategory(string catType)
        {
            return (catType == "EXPENSE" ||
                    catType == "INCOME");
        }

        private bool IsAccount(string catType)
        {
            return (catType == "ASSET" ||
                    catType == "CREDIT" ||
                    catType == "BANK" ||
                    catType == "LIABILITY");
        }

        private string QifCategoryType(string catType)
        {
            var qifCatType = catType == "INCOME" ? "I" :
                             catType == "EXPENSE" ? "E" :
                             "?";

            if (qifCatType == "?")
            {
                OnLogEvent("WARNING", $"Unknown category type: {catType}");
            }

            return qifCatType;
        }

        private string QifAccountType(string accType)
        {
            var qifAccType = accType == "BANK" ? "Bank" :
                             accType == "CREDIT" ? "CCard" :
                             accType == "ASSET" ? "Oth A" :
                             accType == "LIABILITY" ? "Oth L" :
                             "?";

            if (qifAccType == "?")
            {
                OnLogEvent("WARNING", $"Unknown account type: {accType}");
            }

            return qifAccType;
        }
    }
}
