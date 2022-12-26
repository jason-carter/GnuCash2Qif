using System;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class UnknownAccountException: Exception
    {
        public UnknownAccountException(string transactionGuid)
            : base($"Unknown account for transaction {transactionGuid}! This will be ignored in the export") { }
    }
}
