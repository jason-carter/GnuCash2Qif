using CommandLine;
using GnuCash.Sql2Qif.Library;
using GnuCash.Sql2Qif.Library.DAL.Readers;
using System;
using System.IO;

namespace GnuCashSql2Qif
{
    class GnuCashSql2Qif
    {
        static void Main(string[] args)
        {
            var progressHandler = new Progress<string>(value =>
            {
                Console.WriteLine(value);
            });
            var progress = progressHandler as IProgress<string>;

            progress?.Report("Running...");

            try
            { 
                Parser.Default
                    .ParseArguments<CommandLineOptions>(args)
                    .WithParsed(a =>
                    {
                        // Check the datasource files exists
                        if (!File.Exists(a.DataSource))
                        {
                            gnuCashLogger.LogError($"ERROR: Datasource '{a.DataSource}' does not exist!");
                            Environment.Exit(-2);
                        }
                        // TODO: check if the dataource is a valid GnuCash Sqlite file
                        // TODO: check the output file doesn't exist, confirm overwrite if it does

                        AccountReaderWithSqliteConnection accReader = new(a.DataSource, progress);
                        TransactionReaderWithSqliteConnection trxReader = new(a.DataSource, progress);
                        using (var writer = File.CreateText(a.Output))
                        {
                            var runExtract = new Exporter(progress, accReader, trxReader, writer);
                            runExtract.Export();
                        }

                        progress?.Report("GnuCashSql2Qif successfully completed.");
                        Environment.Exit(0);
                    })
                    .WithNotParsed(a =>
                    {
                        progress?.Report("Exiting GnuCashSql2Qif because the arguments could not be parsed.");
                        Environment.Exit(-2);
                    });
            }
            catch (Exception ex)
            {
                progress?.Report($"ERROR: Exiting GnuCashSql2Qif because the following error was thrown: {ex.Message}");
                Environment.Exit(-2);
            }
        }
    }
}
