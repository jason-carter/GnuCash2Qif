using System.Collections.Generic;
using System.Data.SQLite;
using GnuCash.Sql2Qif.Library.BLL;
using Microsoft.Extensions.Logging;

namespace GnuCash.Sql2Qif.Library.DAL
{
    public class SqlLiteAccountDAO : IAccountDAO
    {
        private readonly ILogger logger;

        public SqlLiteAccountDAO(ILogger<SqlLiteAccountDAO> logger)
        {
            this.logger = logger;
        }

        public IDictionary<string, IAccount> Extract(string dataSource)
        {
            var connectionString = string.Format("DataSource={0}", dataSource);
            var accounts = new Dictionary<string, IAccount>();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var reader = new SQLiteCommand(SqlQueries.SqlGetAccountsAndCategoryHiearchy, conn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var acc = new Account(reader["guid"].ToString(),
                                              reader["path"].ToString(),
                                              reader["description"].ToString(),
                                              reader["account_type"].ToString());
                        accounts.Add(acc.Guid, acc);
                    }
                }
            }

            return accounts;
        }
    }
}
