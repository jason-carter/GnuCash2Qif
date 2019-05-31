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
            OnLogEvent("INFO", "Running...");

            var accDAO = new AccountDAO();
            List<IAccount> accounts = accDAO.Extract(dataSource).ToList<IAccount>();

            Console.WriteLine("!Type:Cat"); // Start of Category Section
            accounts.FindAll(n => n.AccountType == "EXPENSE" || n.AccountType == "INCOME")
                .ToList().ForEach(n =>Console.Write(n.QifOutput()));

            Console.WriteLine("!Option:AutoSwitch"); // TODO: Check what this does6

            Console.WriteLine("!Account"); // Start or Account Section
            accounts.FindAll(n => n.AccountType == "ASSET" || n.AccountType == "CREDIT" || n.AccountType == "BANK")
                .ToList().ForEach(n => Console.Write(n.QifOutput()));

            // Extract the Transactions

            var trxDAO = new TransactionDAO();
            List<ITransaction> transactions = trxDAO.Extract(dataSource, accounts).ToList<ITransaction>();

            // Output the Transactions
        }
    }
}
