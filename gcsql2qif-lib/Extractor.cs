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
        public IAccountDAO accDAO;
        public ITransactionDAO trxDAO;
        public ICashOutputter cashOutputter;

        public Extractor(IAccountDAO accDAO, ITransactionDAO trxDAO, ICashOutputter cashOutputter)
        {
            this.accDAO = accDAO;
            this.trxDAO = trxDAO;
            this.cashOutputter = cashOutputter;
        }

        private void OnLogEvent(string level, string logMessage)
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
            List<IAccount> accounts = accDAO.Extract(dataSource).ToList<IAccount>();

            // TODOO: Log some useful information like number of accounts, number of type of accounts

            OnLogEvent("INFO", "Extracting transactions...");
            List<ITransaction> transactions = trxDAO.Extract(dataSource, accounts).ToList<ITransaction>();
            // TODO: Log some useful information like number of transactions, number of transactions per account

            cashOutputter.LogEvent += HandleLogEvent;
            cashOutputter.Write(accounts, outputFileName);
        }

        private void HandleLogEvent(object sender, LogEventArgs args)
        {
            OnLogEvent(args.LogLevel, args.LogMessage);
        }
    }
}
