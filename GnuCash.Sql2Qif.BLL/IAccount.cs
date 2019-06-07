using System;
using System.Collections.Generic;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public interface IAccount
    {
        string Guid { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string AccountType { get; set; }
        string Hierarchy { get; set; }
        int HierarchyLevel { get; set; }
        List<ITransaction> Transactions { get; set; }

        string QifAccountOutput();
    }
}
