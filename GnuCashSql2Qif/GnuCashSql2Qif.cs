using CommandLine;
using GnuCash.Sql2Qif.Library;
using GnuCash.Sql2Qif.Library.DAL.Readers;
using GnuCash.Sql2Qif.Library.DTO;
using GnuCash.Sql2Qif.Library.Outputters;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

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

                        AccountReaderWithSqliteConnection accReader = new(a.DataSource, loggerFactory.CreateLogger<Account>());
                        TransactionReaderWithSqliteConnection trxReader = new(a.DataSource, loggerFactory.CreateLogger<Transaction>());
                        using (var writer = File.CreateText(a.Output))
                        {
                            var runExtract = new Exporter(loggerFactory.CreateLogger<Account>(), accReader, trxReader, writer);
                            runExtract.Export();
                        }

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
