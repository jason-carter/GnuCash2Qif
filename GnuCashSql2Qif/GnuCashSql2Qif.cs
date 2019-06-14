using System;
using GnuCash.Sql2Qif.Library;
using CommandLine;

namespace GnuCashSql2Qif
{
    class GnuCashSql2Qif
    {
        static void Main(string[] args)
        {
            try { 
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(a =>
                    {
                        // TODO: check the datasource files exists
                        // TODO: check if the dataource is a valid GnuCash Sqlite file

                        // TODO: check the output file doesn't exist, confirm overwrite if it does

                        var runExtract = new Extractor();
                        runExtract.LogEvent += HandleLogEvent;

                        runExtract.ExtractData(a.DataSource, a.Output);

                        WriteLog("INFO", "GnuCashSql2Qif successfully completed.");
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
                WriteLog("ERROR", $"Exiting GnuCashSql2Qif because the following error was thrown: {ex.Message}");
                Environment.Exit(-2);
            }
        }

        static void WriteLog(string level, string message)
        {
            Console.WriteLine($"{DateTime.Now} {level} {message}");
        }

        static void HandleLogEvent(object sender, LogEventArgs args)
        {
            WriteLog(args.LogLevel, args.LogMessage);
        }
    }
}
