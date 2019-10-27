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
                where acc.account_type in ('ASSET', 'CREDIT', 'BANK', 'LIABILITY')
            )
            select
                            t.guid              as TrxGuid,
                            acc.guid            as AccGuid,
                            acc.name            as AccountName,
                            t.post_date         as DatePosted,
                            t.Num               as Ref,
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
                        var trxValue = Convert.ToDecimal(reader["trxValue"].ToString());
                        var isReconciled = reader["isReconciled"].ToString();

                        AddAccountSplit(accountGuid, trx, accounts, trxValue, isReconciled);
                        AddAccountSplit(categoryGuid, trx, accounts, trxValue);

                        trx.DatePosted = Convert.ToDateTime(reader["DatePosted"].ToString());
                        trx.Ref = reader["Ref"].ToString();
                        trx.Description = reader["Description"].ToString();
                        trx.Memo = reader["Notes"].ToString();
                    }
                }
            }

            return transactions;
        }

        private Transaction AddTransaction(string trxGuid, List<ITransaction> transactions)
        {
            var trx = (Transaction) transactions.Where<ITransaction>(t => t.TransactionGuid == trxGuid).FirstOrDefault<ITransaction>(); // should only be one trx entry

            if (trx == null)
            {
                trx = new Transaction
                {
                    TransactionGuid = trxGuid
                };

                // new transaction, so add it to the transaction list
                transactions.Add(trx);
            }

            return trx;
        }

        private void AddAccountSplit(string accountGuid, Transaction trx, IEnumerable<IAccount> accounts, decimal trxValue, string isReconciled = "n")
        {
            if (accountGuid.Equals(string.Empty))
            {
                return;
            }

            // Transactions belong to one or more accounts, and may represent a transfer between
            // accounts, in which case it will have two account references

            var account = accounts.Where<IAccount>(a => a.Guid == accountGuid).FirstOrDefault<IAccount>();

            if (account == null)    
            {
                // This should never happen unless there's a problem with the gnucash database
                throw new Exception($"Unknown account on transaction {accountGuid}, could be due to corrupt GnuCash database");
                //TODO: If this is a problem we may well be able to simply ignore it and return from function without an error...
            }

            // Wrap the account up into a split to represent double entry accounting
            IAccountSplit accSplit = new AccountSplit()
            {
                Account = account,
                Reconciled = isReconciled,
                Trxvalue = trxValue
            };

            if (!IsCategory(account.AccountType))
            {
                account.Transactions.Add(trx);      // Transaction is added to the parent account...
                trx.ParentAccounts.Add(account);    // ... and is added to parent account reference of the transaction
            }
            trx.AccountSplits.Add(accSplit);      // The account split is added to the transaction reference
        }

         private bool IsCategory(string catType)
        {
            return (catType == "EXPENSE" || catType == "INCOME");
        }
    }
}
