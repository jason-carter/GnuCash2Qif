using System;
using GnuCash.Sql2Qif.Library.BLL;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.DAL
{
    public class SqlLiteTransactionDAO : ITransactionDAO
    {
        private readonly ILogger logger;

        public SqlLiteTransactionDAO(ILogger<SqlLiteTransactionDAO> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<ITransaction> Extract(string dataSource, IEnumerable<IAccount> accounts)
        {
            var connectionString = string.Format("DataSource={0}", dataSource);
            var transactions = new List<ITransaction>();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using var reader = new SQLiteCommand(SqlQueries.SqlGetTransactions, conn).ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        var trx = new Transaction(reader["TrxGuid"].ToString(),
                                                  reader["AccGuid"].ToString(),
                                                  reader["AccountName"].ToString(),
                                                  Convert.ToDateTime(reader["DatePosted"].ToString()),
                                                  reader["Ref"].ToString(),
                                                  reader["Description"].ToString(),
                                                  reader["Notes"].ToString(),
                                                  reader["isReconciled"].ToString(),
                                                  Convert.ToDecimal(reader["trxValue"].ToString()),
                                                  accounts);

                        var existingTrx = (Transaction)transactions.Where<ITransaction>(t => t.TransactionGuid == trx.TransactionGuid).FirstOrDefault<ITransaction>();

                        if (existingTrx != null)
                        {
                            // Merge current transaction with the existing one. Since this is double entry bookkeeping
                            // there is usually one account entry and one (or could be more) category entry
                            if (trx.IsAccountSide)
                            {
                                // Record the account rather than category details on the transaction
                                existingTrx.AccountGuid = trx.AccountGuid;
                                existingTrx.AccountName = trx.AccountName;
                            }

                            if (trx.AccountSplits.Count > 0)
                            {
                                // Should always be one split per transaction unless added here
                                existingTrx.AccountSplits.AddRange(trx.AccountSplits);
                            }

                            if (existingTrx.AccountSplits.Count > 2)
                            {
                                logger.LogWarning($"Multiple splits for {existingTrx}, but only two splits currently supported!");
                            }

                            // Update the accounts reference with the existing_trx
                            var parentAcc = accounts.Where<IAccount>(a => a.Guid == existingTrx.AccountGuid).First<IAccount>();
                            var oldTrx = parentAcc.Transactions.Where<ITransaction>(t => t.TransactionGuid == existingTrx.TransactionGuid).FirstOrDefault<ITransaction>();
                            if (oldTrx != null)
                            {
                                parentAcc.Transactions.Remove(oldTrx);
                            }
                            parentAcc.Transactions.Add(existingTrx);
                        }
                        else
                        {
                            transactions.Add(trx);
                        }
                    }
                    catch (UnknownAccountException uae)
                    {
                        logger.LogWarning(uae.Message);
                    }
                }
            }

            return transactions;
        }
    }
}
