using System;
using System.Collections.Generic;
using System.Text;
using GnuCash.Sql2Qif.Library.DAL;
using GnuCash.Sql2Qif.Library.BLL;
using System.Linq;

namespace GnuCash.Sql2Qif.Library
{
    public class Extractor
    {
        public void ExtractData(string dataSource, string outputFileName)
        {
            var accDAO = new AccountDAO();
            List<IAccount> accounts = accDAO.Extract(dataSource).ToList<IAccount>();

            Console.WriteLine("!Type:Cat"); // Start of Category Section
            accounts.FindAll(n => n.AccountType == "EXPENSE" || n.AccountType == "INCOME")
                .ToList().ForEach(n =>Console.Write(n.QifOutput()));

            Console.WriteLine("!Option:AutoSwitch"); // TODO: Check what this does6

            Console.WriteLine("!Account"); // Start or Account Section
            accounts.FindAll(n => n.AccountType == "ASSET" || n.AccountType == "CREDIT" || n.AccountType == "BANK")
                .ToList().ForEach(n => Console.Write(n.QifOutput()));

            // Extract the Transactions

            // Output the Transactions
        }

    }
}
