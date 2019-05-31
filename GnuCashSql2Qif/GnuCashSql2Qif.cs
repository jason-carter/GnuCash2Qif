using System;
using GnuCash.Sql2Qif.Library;
using CommandLine;

namespace GnuCashSql2Qif
{
    class GnuCashSql2Qif
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(a =>
                    {
                        // TODO: check the datasource files exists
                        // TODO: check the output file doesn't exist, confirm overwrite if it does?

                        var runExtract = new Extractor();
                        runExtract.ExtractData(a.DataSource, a.Output);

                        Environment.Exit(0);
                    })
                    .WithNotParsed(a =>
                    {
                        Console.WriteLine("Exiting GnuCashSql2Qif. Arguments could not be parsed.");
                        Environment.Exit(-2);
                    });
        }
    }
}
