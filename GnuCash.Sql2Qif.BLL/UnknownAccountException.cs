using System;

namespace GnuCash.Sql2Qif.Library.DTO
{
    public class UnknownAccountException: Exception
    {
        public UnknownAccountException(string transactionGuid)
            : base($"Unknown account for transaction {transactionGuid}! This will be ignored in the export") { }
    }
}
