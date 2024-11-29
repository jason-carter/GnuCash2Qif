using CommunityToolkit.Mvvm.Input;
using GnuCash.Sql2Qif.Library;
using GnuCash.Sql2Qif.Library.DAL.Readers;
using GnuCash.Sql2Qif.Library.DTO;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace GnuCash2QifGui
{
    public class MainWindowViewModel : ObservableObject
    {
        private string _dataSource;
        private string _outputFile;

        public string DataSource
        {
            get => this._dataSource;

            set
            {
                this._dataSource = value;
                this.OnPropertyChanged(nameof(this._dataSource));
                this.OnPropertyChanged(nameof(this.IsExportEnabled));
            }
        }

        public string OutputFile
        {
            get => this._outputFile;

            set
            {
                this._outputFile = value;
                this.OnPropertyChanged(nameof(this._outputFile));
                this.OnPropertyChanged(nameof(this.IsExportEnabled));
            }
        }

        public bool IsExportEnabled
        {
            // Export is only allowed (Export button enabled) if the datasource and output
            // file fields are populated.
            get => this._dataSource?.Trim().Length > 0 && this._outputFile?.Trim().Length > 0;
        }

        public AsyncRelayCommand ExportCommand { get; private set; }

        public MainWindowViewModel()
        {
            this.ExportCommand = new AsyncRelayCommand(this.DoExportAsync);
        }


        //private static async Task DoExport(string dataSourcePath, string outputPath, IProgress<int> progress, IProgress<string> conversion, IProgress<string> status)
        private async Task DoExportAsync()
        {
            //var runExtract = new Extractor();
            //runExtract.LogEvent += HandleLogEvent;
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddDebug();
            });
            // var gnuCashLogger = loggerFactory.CreateLogger<GnuCashSql2QifGui>();

            //status.Report($"Running...");
            AccountReaderWithSqliteConnection accReader = new(this.DataSource, loggerFactory.CreateLogger<Account>());
            TransactionReaderWithSqliteConnection trxReader = new(this.DataSource, loggerFactory.CreateLogger<Transaction>());

            using var writer = File.CreateText(this.OutputFile);

            var runExtract = new Exporter(loggerFactory.CreateLogger<Account>(), accReader, trxReader, writer);
            //runExtract.Export();
            await Task.Run(() => runExtract.Export());

            //await Task.Run(() => runExtract.ExtractData(dataSourcePath, outputPath));
            //status.Report("GnuCashSql2Qif successfully completed.");
        }

        //static void WriteLog(string level, string message)
        //{
        //    //statusReport.Content = $"{DateTime.Now} {level} {message}";
        //    Console.WriteLine($"{DateTime.Now} {level} {message}");
        //}

        //static void HandleLogEvent(object sender, LogEventArgs args)
        //{
        //    WriteLog(args.LogLevel, args.LogMessage);
        //}
    }
}
