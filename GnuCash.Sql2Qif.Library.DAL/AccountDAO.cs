using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using GnuCash.Sql2Qif.Library.BLL;

namespace GnuCash.Sql2Qif.Library.DAL
{
    public class AccountDAO
    {
        readonly string sql = @"
            with recursive cteAccounts(guid, name, account_type, parent_guid, code, description, hidden, placeholder, level, path) AS
            (
                select guid, name, account_type, parent_guid, code,
                        description, hidden, placeholder, 0, ''
                from accounts
                where parent_guid is null
                and name = 'Root Account'
                union all
                select a.guid, a.name, a.account_type, a.parent_guid, a.code,
                            a.description, a.hidden, a.placeholder, p.level + 1, p.path || ':' || a.name
                from        accounts a
                inner join  cteAccounts p on p.guid = a.parent_guid
                order by 9 desc -- by using desc we're doing a depth-first search
            )
            select substr('                                        ', 1, level* 10) || name 'hierarchy',
                    name,
                    guid,
                    description,
                    substr(path, 2, length(path)) 'path',
                    account_type, level
            from    cteAccounts
            where account_type in ('ASSET', 'CREDIT', 'BANK', 'EXPENSE', 'INCOME')
            ";

        public IEnumerable<IAccount> Extract(string dataSource)
        {
            var connectionString = string.Format("DataSource={0}", dataSource);
            var accounts = new List<IAccount>();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var reader = new SQLiteCommand(sql, conn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var accountType = reader["account_type"].ToString();

                        switch (accountType)
                        {
                            case "ASSET":
                            case "CREDIT":
                            case "BANK":
                                accounts.Add(new Account
                                {
                                    Guid = reader["guid"].ToString(),
                                    Name = reader["path"].ToString(),
                                    Description = reader["description"].ToString(),
                                    AccountType = reader["account_type"].ToString(),
                                    Hierarchy = reader["hierarchy"].ToString()
                                });
                                break;
                            case "EXPENSE":
                            case "INCOME":
                                accounts.Add(new Category
                                {
                                    Guid = reader["guid"].ToString(),
                                    Name = reader["path"].ToString(),
                                    Description = reader["description"].ToString(),
                                    AccountType = reader["account_type"].ToString(),
                                    Hierarchy = reader["hierarchy"].ToString()
                                });
                                break;
                        }
                    }
                }
            }

            return accounts;
        }
    }
}
