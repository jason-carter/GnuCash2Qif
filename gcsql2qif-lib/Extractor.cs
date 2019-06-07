using System;
using System.Collections.Generic;
using System.Text;
using GnuCash.Sql2Qif.Library.DAL;
using GnuCash.Sql2Qif.Library.BLL;
using System.Linq;

namespace GnuCash.Sql2Qif.Library
{
    public class Extractor
    {
        public event EventHandler<LogEventArgs> LogEvent;

        protected void OnLogEvent(string level, string logMessage)
        {
            LogEventArgs args = new LogEventArgs()
            {
                LogLevel = level,
                LogMessage = logMessage
            };
            LogEvent?.Invoke(this, args);
        }

        public void ExtractData(string dataSource, string outputFileName)
        {
            OnLogEvent("INFO", "Extracting accounts...");
            List<IAccount> accounts = (new AccountDAO()).Extract(dataSource).ToList<IAccount>();

            // TODOO: Log some useful information like number of accounts, number of type of accounts

            OnLogEvent("INFO", "Extracting transactions...");
            List<ITransaction> transactions = (new TransactionDAO()).Extract(dataSource, accounts).ToList<ITransaction>();

            // TODO: Log some useful information like number of transactions, number of transactions per account

            OutputQifData(accounts, outputFileName);
        }

        private void OutputQifData(List<IAccount> accounts, string outputFileName)
        {
            // Category section (expense / income accounts)
            Console.WriteLine("!Type:Cat");
            accounts.FindAll(n => n.AccountType == "EXPENSE" || n.AccountType == "INCOME")
                .ToList().ForEach(n => Console.Write(n.QifAccountOutput()));

            Console.WriteLine("!Option:AutoSwitch"); // TODO: Check what this does6

            // Account section (asset / credit / bank accounts
            Console.WriteLine("!Account"); // Start or Account Section
            accounts.FindAll(n => n.AccountType == "ASSET" || n.AccountType == "CREDIT" || n.AccountType == "BANK")
                .ToList().ForEach(n => Console.Write(n.QifAccountOutput()));

            // Transaction section by account
            accounts.FindAll(n => n.AccountType == "ASSET" || n.AccountType == "CREDIT" || n.AccountType == "BANK")
                .ToList().ForEach(n => Console.Write(((Account) n).QifAccountTransactionOutput()));
        }
    }
}
