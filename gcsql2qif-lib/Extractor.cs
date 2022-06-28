using System.Collections.Generic;
using GnuCash.Sql2Qif.Library.DAL;
using GnuCash.Sql2Qif.Library.BLL;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GnuCash.Sql2Qif.Library
{
    public class Extractor
    {
        //public event EventHandler<LogEventArgs> LogEvent;
        private readonly ILogger logger;
        private readonly IAccountDAO accDAO;
        private readonly ITransactionDAO trxDAO;

        public Extractor(ILogger<Extractor> logger, IAccountDAO accDAO, ITransactionDAO trxDAO)
        {
            this.logger = logger;
            this.accDAO = accDAO;
            this.trxDAO = trxDAO;
        }

        public List<IAccount> ExtractData(string dataSource)
        {
            logger.LogInformation("Extracting accounts...");
            List<IAccount> accounts = accDAO.Extract(dataSource).ToList<IAccount>();

            // TODOO: Log some useful information like number of accounts, number of type of accounts

            logger.LogInformation("Extracting transactions...");
            List<ITransaction> transactions = trxDAO.Extract(dataSource, accounts).ToList<ITransaction>();
            // TODO: Log some useful information like number of transactions, number of transactions per account

            return accounts;
        }
    }
}
