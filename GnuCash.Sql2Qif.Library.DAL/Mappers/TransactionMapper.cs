using GnuCash.Sql2Qif.Library.DTO;
using System;
using System.Collections.Generic;
using System.Data;


namespace GnuCash.Sql2Qif.Library.DAL.Mappers
{
    class TransactionMapper : MapperBase<string, ITransaction>
    {
        public TransactionMapper(IProgress<string> progress) : base(progress) { }

        protected override KeyValuePair<string, ITransaction> Map(IDataRecord record)
        {
            Transaction trx = new(record["TrxGuid"].ToString(),
                                  record["AccGuid"].ToString(),
                                  record["AccountName"].ToString(),
                                  Convert.ToDateTime(record["DatePosted"].ToString()),
                                  record["Ref"].ToString(),
                                  record["Description"].ToString(),
                                  record["Notes"].ToString(),
                                  record["isReconciled"].ToString(),
                                  Convert.ToDecimal(record["trxValue"].ToString()));

            string key = $"{trx.TransactionGuid}:{trx.AccountGuid}";
            return new KeyValuePair<string, ITransaction>(key, trx);
        }
    }
}
