using GnuCash.Sql2Qif.Library.DAL.Readers;
using GnuCash.Sql2Qif.Library.DTO;
using GnuCash.Sql2Qif.Library.Outputters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library
{
    public class Exporter
    {
        private readonly ILogger logger;
        private readonly ReaderBase<string, IAccount> accReader;
        private readonly ReaderBase<string, ITransaction> trxReader;

        private readonly QifOutputterBase<IAccount> accountWriter;
        private readonly QifOutputterBase<IAccount> categoryWriter;
        private readonly QifOutputterBase<IAccount> transactionWriter;

        public Exporter(ILogger<IAccount> logger, ReaderBase<string, IAccount> accReader, ReaderBase<string, ITransaction> trxReader, StreamWriter outputter)
        {
            this.logger = logger;
            this.accReader = accReader;
            this.trxReader = trxReader;

            accountWriter = new QifAccountOutputter(logger, outputter);
            categoryWriter = new QifCategoryOutputter(logger, outputter);
            transactionWriter = new QifTransactionOutputter(logger, outputter);
        }

        public void Export()
        {
            logger.LogInformation("Extracting all accounts...");
            IDictionary<string, IAccount> accounts = accReader.Execute();

            logger.LogInformation("Extracting list of transactions...");
            IDictionary<string, ITransaction> transactions = trxReader.Execute();


            logger.LogInformation("Assigning transactions to their accounts...");
            accounts.Values.ToList().ForEach(acc => acc.Transactions = transactions.Values.Where(t => t.AccountGuid == acc.Guid).ToList());

            logger.LogInformation("Add transaction account references...");
            transactions.Values.ToList().ForEach(trx => trx.AccountReference = SetAccountReference(trx, transactions, accounts));

            logger.LogInformation("Writing to output...");
            logger.LogInformation("Writing category section...");
            categoryWriter.Write(accounts);
            logger.LogInformation("Writing accounts section...");
            accountWriter.Write(accounts);
            logger.LogInformation("Writing transactions by accounts...");
            transactionWriter.Write(accounts);
        }

        private string SetAccountReference(ITransaction trx, 
                                           IDictionary<string, ITransaction> transactions,
                                           IDictionary<string, IAccount> accounts)
        {
            // TODO: Major perfomance issue. The transaction/account lookups are slow as they're not using the dictionary
            //       due to the key being trxGuid:accGuid. There are two entries per transaction (double entry book keeping)
            //       but we need the other part of the transaction and we don't know the key.

            // only uses the first account split and ignore any others
            var trxSplit = transactions.Values.Where(t => t.TransactionGuid == trx.TransactionGuid && t.AccountGuid != trx.AccountGuid).FirstOrDefault();

            if (trxSplit == null)
            {
                logger.LogWarning($"Could not find the account split for ({trx})");
                return "";
            }

            // using the split, look up the account name, which will be the reference on the transaction
            IAccount trxSplitParent = accounts[trxSplit.AccountGuid];

            if (trxSplitParent == null) return "";

            if (trxSplitParent.IsAccount) return $"[{trxSplitParent.Name}]";

            return trxSplitParent.Name;
        }
    }
}
