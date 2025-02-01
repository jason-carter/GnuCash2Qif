using GnuCash.Sql2Qif.Library.DAL.Mappers;
using GnuCash.Sql2Qif.Library.DTO;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace GnuCash.Sql2Qif.Library.DAL.Readers
{
    public class TransactionReaderWithSqliteConnection : ReaderWithSqliteConnection<string, ITransaction>
    {
        private readonly IProgress<string> progress;

        public TransactionReaderWithSqliteConnection(string datasource, IProgress<string> progress) : base(datasource)
        {
            this.progress = progress;
        }

    protected override string CommandText => SqlQueries.SqlGetTransactions;

        protected override CommandType CommandType => CommandType.Text;

        protected override MapperBase<string, ITransaction> GetMapper() => new TransactionMapper(progress);

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
