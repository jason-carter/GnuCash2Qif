using GnuCash.Sql2Qif.Library.DTO;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;

namespace GnuCash.Sql2Qif.Library.DAL.Mappers
{
    class AccountMapper : MapperBase<string, IAccount>
    {
        public AccountMapper(ILogger<IAccount> logger) : base(logger) { }

        protected override KeyValuePair<string, IAccount> Map(IDataRecord record)
        {
            Account acc = new(record["guid"].ToString(),
                              record["path"].ToString(),
                              record["description"].ToString(),
                              record["account_type"].ToString());
            return new KeyValuePair<string, IAccount>(acc.Guid, acc);
        }
    }
}
