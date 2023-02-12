using GnuCash.Sql2Qif.Library.DAL.Mappers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace GnuCash.Sql2Qif.Library.DAL.Readers
{
    abstract public class ReaderBase<TKey, TValue>
    {
        protected abstract IDbConnection GetConnection();
        protected abstract string CommandText { get; }
        protected abstract CommandType CommandType { get; }
        protected abstract Collection<IDataParameter> GetParameters(IDbCommand command);
        protected abstract MapperBase<TKey, TValue> GetMapper();

        public Dictionary<TKey, TValue> Execute()
        {
            Dictionary<TKey, TValue> dict = new();
            using (IDbConnection connection = GetConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = CommandText;
                command.CommandType = CommandType;
                foreach (IDataParameter param in GetParameters(command))
                    command.Parameters.Add(param);

                try
                {
                    connection.Open();
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            MapperBase<TKey, TValue> mapper = GetMapper();
                            dict = mapper.MapAll(reader);
                            return dict;
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
