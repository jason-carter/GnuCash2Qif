using GnuCash.Sql2Qif.Library.DAL.Readers;
using GnuCash.Sql2Qif.Library.DTO;
using GnuCash.Sql2Qif.Library.Outputters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuCash.Sql2Qif.Library
{
    public class Exporter
    {
        private readonly IProgress<string> _progress;
        private readonly ReaderBase<string, IAccount> _accReader;
        private readonly ReaderBase<string, ITransaction> _trxReader;

        private readonly QifOutputterBase<IAccount> _accountWriter;
        private readonly QifOutputterBase<IAccount> _categoryWriter;
        private readonly QifOutputterBase<IAccount> _transactionWriter;

        public Exporter(IProgress<string> progress, ReaderBase<string, IAccount> accReader, ReaderBase<string, ITransaction> trxReader, StreamWriter outputter)
        {
            this._progress = progress;
            this._accReader = accReader;
            this._trxReader = trxReader;

            _accountWriter = new QifAccountOutputter(progress, outputter);
            _categoryWriter = new QifCategoryOutputter(progress, outputter);
            _transactionWriter = new QifTransactionOutputter(progress, outputter);
        }

        public void Export()
        {
            _progress?.Report("Extracting all accounts...");
            IDictionary<string, IAccount> accounts = _accReader.Execute();

            _progress?.Report("Extracting list of transactions...");
            IDictionary<string, ITransaction> transactions = _trxReader.Execute();


            _progress?.Report("Assigning transactions to their accounts...");
            accounts.Values.ToList().ForEach(acc => acc.Transactions = transactions.Values.Where(t => t.AccountGuid == acc.Guid).ToList());

            _progress?.Report("Add transaction account references...");
            transactions.Values.ToList().ForEach(trx => trx.AccountReference = SetAccountReference(trx, transactions, accounts));

            _progress?.Report("Writing to output...");
            _progress?.Report("Writing category section...");
            _categoryWriter.Write(accounts);
            _progress?.Report("Writing accounts section...");
            _accountWriter.Write(accounts);
            _progress?.Report("Writing transactions by accounts...");
            _transactionWriter.Write(accounts);
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
                _progress?.Report($"WARNING: Could not find the account split for ({trx})");
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
