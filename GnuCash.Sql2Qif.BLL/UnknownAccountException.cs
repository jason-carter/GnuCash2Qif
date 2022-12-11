using System;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class UnknownAccountException: Exception
    {
        public UnknownAccountException(string transactionGuid)
            : base($"Unknown account on transaction {transactionGuid}, could be due to corrupt GnuCash database") { }
    }
}
