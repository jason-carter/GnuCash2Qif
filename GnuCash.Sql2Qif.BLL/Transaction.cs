﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GnuCash.Sql2Qif.Library.BLL
{
    public class Transaction : ITransaction
    {
        public Transaction()
        {
            Categories = new List<IAccount>();
        }

        public string TransactionGuid { get; set; }
        public DateTime DatePosted { get; set; }
        public string Ref { get; set; }
        public string Description { get; set; }
        public string Memo { get; set; }
        public List<IAccount> Categories { get; set; }
        public string Reconciled { get; set; }
        public decimal Value { get; set; }
    }
}
