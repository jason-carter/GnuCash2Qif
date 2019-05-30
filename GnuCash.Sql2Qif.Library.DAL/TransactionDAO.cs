using System;
using GnuCash.Sql2Qif.Library.BLL;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Globalization;

namespace GnuCash.Sql2Qif.Library.DAL
{
    public class TransactionDAO
    {
        readonly string sql = @"
            with recursive cteCategories(guid, name, account_type, parent_guid, code, description, hidden, placeholder, level, path) AS
            (
                select  guid, name, account_type, parent_guid, code,
                        description, hidden, placeholder, 0, ''
                from    accounts 
                where   parent_guid is null
                and     name = 'Root Account'
                union all
                select      a.guid, a.name, a.account_type, a.parent_guid, a.code,
                            a.description, a.hidden, a.placeholder, p.level + 1, p.path || ':' || a.name
                from        accounts a
                inner join  cteCategories p on p.guid = a.parent_guid
                where       a.account_type in ('EXPENSE', 'INCOME')
                order by 9 desc -- by using desc we're doing a depth-first search
            ),
            cteAccounts(guid, name, account_type, description) as
            (
                select      acc.guid, acc.name, acc.account_type, acc.description
                from        accounts  acc
                inner join  accounts  p   on p.guid = acc.parent_guid
                                          and p.parent_guid is null
                                          and p.account_type = 'ROOT'
                                          and p.name = 'Root Account'
                where acc.account_type in ('ASSET', 'CREDIT', 'BANK')
            )
            select
                            t.guid              as TrxGuid,
                            acc.guid            as AccGuid,
                            acc.name            as AccountName,
                            t.post_date         as DatePosted,
                            t.Num,
                            t.Description,
                            sl.string_val       as Notes,
                            cat.guid            as CategoryGuid,
                            cat.name            as Transfer,
                            s.reconcile_state   as isReconciled,
                            case acc.account_type
                                when 'EQUITY' then ROUND((s.value_num / -100.0), 2)
                                else ROUND((s.value_num / 100.0), 2)
                            end as trxValue
            from            splits        as s
            inner join      transactions  as t    on t.guid = s.tx_guid
            left outer join cteAccounts   as acc  on acc.guid = s.account_guid
            left outer join cteCategories as cat  on cat.guid = s.account_guid
            left outer join slots         as sl   on sl.obj_guid = t.guid and sl.name = 'notes'
            order by        t.guid,
                            t.post_date asc
            ";

        public IEnumerable<ITransaction> Extract(string dataSource, IEnumerable<IAccount> accounts)
        {
            var connectionString = string.Format("DataSource={0}", dataSource);
            var transactions = new List<ITransaction>();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var reader = new SQLiteCommand(sql, conn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var trx = AddTransaction(reader["TrxGuid"].ToString(), transactions);

                        // Lookup the accounts or Categories and add object references
                        var accountGuid = reader["AccGuid"]?.ToString();
                        var categoryGuid = reader["CategoryGuid"]?.ToString();

                        AddTransactionToAccount(accountGuid, trx, accounts);
                        AddCategoryToTransaction(categoryGuid, trx, accounts);

                        var transfer = reader["Transfer"].ToString(); // TODO: This is the category name, do we need this if we've looked up the object?
                        
                        trx.DatePosted = Convert.ToDateTime(reader["DatePosted"].ToString());
                        trx.Number = reader["Num"].ToString(); //TODO: This may not be a number, could be Withd, POS, ATM - do we want those?
                        trx.Description = reader["Description"].ToString();
                        trx.Memo = reader["Notes"].ToString();
                        trx.Reconciled = reader["isReconciled"].ToString();

                        if (!accountGuid.Equals(string.Empty) && categoryGuid.Equals(string.Empty))
                        {
                            // Only using the value against the account entry so the sign is correct for the account.
                            // If we ever extend this beyond the QIF format it will need to store the value for the
                            // category as well, and so keep both (or more) double entry values
                            trx.Value = Convert.ToDecimal(reader["trxValue"].ToString());
                        }
                    }
                }
            }

            return transactions;
        }

        private Transaction AddTransaction(string trxGuid, List<ITransaction> transactions)
        {
            var trx = new Transaction
            {
                TransactionGuid = trxGuid
            };

            var trxLookup = transactions.Where<ITransaction>(t => t.TransactionGuid == trxGuid).FirstOrDefault<ITransaction>(); // should only be one trx entry

            if (trxLookup != null)
            {
                // transaction already on the list, so use the exisitng reference
                trx = (Transaction)trxLookup;
            }
            else
            {
                // new transaction, so add it to the transaction list
                transactions.Add(trx);
            }

            return trx;
        }

        private void AddTransactionToAccount(string accountGuid, Transaction trx, IEnumerable<IAccount> accounts)
        {
            if (accountGuid.Equals(string.Empty))
            {
                return;
            }

            var account = accounts.Where<IAccount>(a => a.Guid == accountGuid).FirstOrDefault<IAccount>();

            if (account != null)
            {
                account.Transactions.Add(trx);
            }
            else
            {
                //TODO: Report WARNING Unknown account on transaction {Guid}
            }
        }

        private void AddCategoryToTransaction(string categoryGuid, Transaction trx, IEnumerable<IAccount> categories)
        {
            if (categoryGuid.Equals(string.Empty))
            {
                return;
            }

            var category = categories.Where<IAccount>(c => c.Guid == categoryGuid).FirstOrDefault<IAccount>();

            if (category != null)
            {
                trx.Categories.Add(category);
            }
            else
            {
                //TODO: Report WARNING Unknown Category on transaction {Guid}
            }
        }
    }
}
