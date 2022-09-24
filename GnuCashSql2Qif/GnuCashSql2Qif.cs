using System;
using GnuCash.Sql2Qif.Library;
using CommandLine;
using GnuCash.Sql2Qif.Library.DAL;
using Microsoft.Extensions.Logging;

namespace GnuCashSql2Qif
{
    class GnuCashSql2Qif
    {
        static void Main(string[] args)
        {
            //var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "yyyy/MM/dd hh:mm:ss ";
            }));
            var gnuCashLogger = loggerFactory.CreateLogger<GnuCashSql2Qif>();

            try
            { 
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(a =>
                    {
                        // TODO: check the datasource files exists
                        // TODO: check if the dataource is a valid GnuCash Sqlite file

                        // TODO: check the output file doesn't exist, confirm overwrite if it does

                        var extractLogger = loggerFactory.CreateLogger<Extractor>();
                        var sqlAccDao = new SqlLiteAccountDAO();
                        var sqlTrxDao = new SqlLiteTransactionDAO();

                        var runExtract = new Extractor(extractLogger, sqlAccDao, sqlTrxDao);

                        var accounts = runExtract.ExtractData(a.DataSource);

                        var qifLogger = loggerFactory.CreateLogger<QifCashOutputter>();
                        var qifOutputter = new QifCashOutputter(qifLogger);
                        qifOutputter.Write(accounts, a.Output);

                        gnuCashLogger.LogInformation("GnuCashSql2Qif successfully completed.");
                        Environment.Exit(0);
                    });
                    // Cleaner output without the error message
                    //.WithNotParsed(a =>
                    //{
                    //    //WriteLog("ERROR", "Exiting GnuCashSql2Qif because the arguments could not be parsed.");
                    //    //Environment.Exit(-2);
                    //});
            }
            catch (Exception ex)
            {
                gnuCashLogger.LogError($"Exiting GnuCashSql2Qif because the following error was thrown: {ex.Message}");
                Environment.Exit(-2);
            }
        }
    }
}
