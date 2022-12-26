using GnuCash.Sql2Qif.Library.BLL;
using GnuCash.Sql2Qif.Library.DAL;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library
{
    public class QifCashOutputter : ICashOutputter
    {
        //public event EventHandler<LogEventArgs> LogEvent;
        private readonly ILogger logger;

        public QifCashOutputter(ILogger<QifCashOutputter> logger)
        {
            this.logger = logger;
        }

        public void Write(IDictionary<string, IAccount> accounts, string outputFileName)
        {
            using (var writer = File.CreateText(outputFileName))
            {
                logger.LogInformation("Writing category section...");
                WriteCategoryList(accounts, writer);
                logger.LogInformation("Writing accounts section...");
                WriteAccountList(accounts, writer);
                logger.LogInformation("Writing transactions by accounts...");
                WriteTransactionListByAccount(accounts, writer);
            }
        }

        private void WriteTransactionListByAccount(IDictionary<string, IAccount> accounts, StreamWriter outputFile)
        {
            outputFile.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list (with transactions this time)

            // Transaction section by account
            accounts.Values.ToList().FindAll(n => n.IsAccount)
                .ToList().ForEach(n => QifAccountTransactionOutput(n, outputFile));
        }

        private void QifAccountTransactionOutput(IAccount acc, StreamWriter output)
        {
            QifAccountTransactionHeaderOutput(acc, output);
            acc.Transactions.Values.ToList().ForEach(t => QifTransactionOutput(acc, t, output));
        }

        private void QifTransactionOutput(IAccount parentAcc, ITransaction trx, StreamWriter output)
        {
            // Use the transaction value of the parent account
            var mainParentAccount = trx.AccountSplits.Where(ac => ac.Account.IsAccount && ac.Account.Guid == parentAcc.Guid).FirstOrDefault();
            var trxValue = mainParentAccount.TrxValue;
            var isReconciled = IsReconciled(mainParentAccount.IsReconciled);
            var accountRef = "";

            if (trx.AccountSplits.Where(ac => ac.Account.IsCategory).Count<IAccountSplit>() > 0)
            {
                // Has one or more category accounts
                // TODO: Consider supporting multiple splits, for now just grab the first one
                accountRef += $"{trx.AccountSplits.Where(ac => ac.Account.IsCategory).FirstOrDefault().Account.Name}";
            }
            else if (trx.AccountSplits.Where(ac => ac.Account.IsAccount).Count<IAccountSplit>() > 1)
            {
                // Has more than one account so is an account transfer
                //TODO: Check if there could be multiple account transfers in a split - tricky
                accountRef += $"[{trx.AccountSplits.Where(ac => ac.Account.IsAccount && ac.Account.Guid != parentAcc.Guid).FirstOrDefault().Account.Name}]";
            }
            else
            {
                logger.LogWarning($"Transaction {trx.ToString()} has no categories and only one account reference ({mainParentAccount.ToString()})");
            }

            output.WriteLine($"D{trx.DatePosted.ToString("MM/d/yyyy")}"); // TODO: Check QIF's supported date formats
            if (trx.Reference != null && !trx.Reference.Equals(string.Empty))
            {
                output.WriteLine($"N{trx.Reference}");
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

        private void WriteAccountList(IDictionary<string, IAccount> accounts, StreamWriter outputFile)
        {
            // Account section (asset / credit / bank / liability accounts)

            outputFile.WriteLine("!Option:AutoSwitch"); // Indicates start of the account list

            outputFile.WriteLine("!Account");
            accounts.Values.ToList().FindAll(n => n.IsAccount)
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

        private void WriteCategoryList(IDictionary<string, IAccount> accounts, StreamWriter outputFile)
        {
            // Category section (expense / income accounts)
            outputFile.WriteLine("!Type:Cat");
            accounts.Values.ToList().FindAll(n => n.IsCategory)
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

        private string QifCategoryType(string catType)
        {
            var qifCatType = catType == "INCOME" ? "I" :
                             catType == "EXPENSE" ? "E" :
                             "?";

            if (qifCatType == "?")
            {
                logger.LogWarning($"Unknown category type: {catType}");
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
                logger.LogWarning($"Unknown account type: {accType}");
            }

            return qifAccType;
        }
    }
}
