using System.Collections.Generic;
using System.Data.SQLite;
using GnuCash.Sql2Qif.Library.BLL;
using System.Linq;

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
                            t.guid,
                            acc.name        as AccountName,
                            t.post_date   as DatePosted,
                            t.Num,
                            t.Description,
                            sl.string_val   as Notes,
                            cat.name as Transfer,
                            s.reconcile_state as isReconciled,
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

        public IEnumerable<ITransaction> Extract (string dataSource, IEnumerable<IAccount> accounts)
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
                        var trx = new Transaction
                        {
                            TransactionGuid = reader["guid"].ToString()
                        };
                        var isExistingTrx = false;

                        // Check for existing transaction in list - use if it exists
                        var trxLookup = transactions.Where<ITransaction>(t => t.TransactionGuid == trx.TransactionGuid).FirstOrDefault<ITransaction>(); // should only be one trx entry

                        if (trxLookup != null)
                        {
                            // transaction already on the list, so use the exisitng reference and enrich it
                            trx = (Transaction) trxLookup;
                            isExistingTrx = true;
                        }

                        // Lookup the account and add transaction to the account's list

                        // Lookup the category and add to the transaction

                        if (!isExistingTrx)
                        {
                            transactions.Add(trx);
                        }
                    }
                }
            }

            return transactions;
        }

    }
}
