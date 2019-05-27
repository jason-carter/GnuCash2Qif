using System;
using GnuCash.Sql2Qif.Library;

namespace GnuCashSql2Qif
{
    class GnuCashSql2Qif
    {
        static void Main(string[] args)
        {

            var dataSource = args[0];
            var outputFileName = args[1];

            var runExtract = new Extractor();
            runExtract.ExtractData(dataSource, outputFileName);

        }
    }
}
