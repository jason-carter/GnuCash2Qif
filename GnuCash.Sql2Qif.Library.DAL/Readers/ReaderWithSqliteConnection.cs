using System.Data;
using System.Data.SQLite;

namespace GnuCash.Sql2Qif.Library.DAL.Readers
{
    abstract public class ReaderWithSqliteConnection<TKey, TValue> : ReaderBase<TKey, TValue>
    {
        private string m_connectionString;

        public ReaderWithSqliteConnection(string datasource)
        {
            m_connectionString = string.Format("DataSource={0}", datasource);
        }

        protected override IDbConnection GetConnection()
        {
            // update to get your connection here  
            IDbConnection connection = new SQLiteConnection(m_connectionString);
            return connection;
        }
    }
}
