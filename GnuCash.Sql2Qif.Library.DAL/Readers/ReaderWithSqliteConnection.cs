using System.Data;
using System.Data.SQLite;

namespace GnuCash.Sql2Qif.Library.DAL.Readers
{
    abstract public class ReaderWithSqliteConnection<TKey, TValue> : ReaderBase<TKey, TValue>
    {
        private readonly string _mConnectionString;

        public ReaderWithSqliteConnection(string datasource)
        {
            _mConnectionString = $"DataSource={datasource}";
        }

        protected override IDbConnection GetConnection()
        {
            // update to get your connection here  
            IDbConnection connection = new SQLiteConnection(_mConnectionString);
            return connection;
        }
    }
}
