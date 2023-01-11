using GnuCash.Sql2Qif.Library.DAL.Mappers;
using GnuCash.Sql2Qif.Library.DTO;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Data;

namespace GnuCash.Sql2Qif.Library.DAL.Readers
{
    public class AccountReaderWithSqliteConnection : ReaderWithSqliteConnection<string, IAccount>
    {
        private readonly ILogger<IAccount> logger;

        public AccountReaderWithSqliteConnection(string datasource, ILogger<IAccount> logger) : base(datasource)
        {
            this.logger = logger;
        }

        protected override string CommandText => SqlQueries.SqlGetAccountsAndCategoryHiearchy;

        protected override CommandType CommandType => CommandType.Text;

        protected override MapperBase<string, IAccount> GetMapper() => new AccountMapper(logger);

        protected override Collection<IDataParameter> GetParameters(IDbCommand command)
        {
            Collection<IDataParameter> collection = new();
            //// USE THIS IF YOU ACTUALLY HAVE PARAMETERS  
            //IDataParameter param1 = command.CreateParameter();  
            //param1.ParameterName = "paramName 1"; // put parameter name here  
            //param1.Value = 5; // put value here;  
            //collection.Add(param1);  
            return collection;
        }
    }
}
