using CommunityToolkit.Mvvm.Input;
using GnuCash.Sql2Qif.Library;
using GnuCash.Sql2Qif.Library.DAL.Readers;
using System;
using System.IO;
using System.Threading.Tasks;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace GnuCash2QifGui
{
    public class MainWindowViewModel : ObservableObject
    {
        private string _dataSource;
        private string _outputFile;
        private string _statusBar;

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

        public string StatusBar
        {
            get => this._statusBar;

            set
            {
                this._statusBar = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsExportEnabled
        {
            // Export is only allowed (Export button enabled) if the datasource and output
            // file fields are populated.
            get => this._dataSource?.Trim().Length > 0
                && this._outputFile?.Trim().Length > 0;
        }

        public AsyncRelayCommand ExportCommand { get; private set; }

        public MainWindowViewModel()
        {
            var progressHandler = new Progress<string>(value =>
            {
                this.StatusBar = value;
            });
            var progress = progressHandler as IProgress<string>;

            this.ExportCommand = new AsyncRelayCommand(() => this.DoExportAsync(progress));

            progress?.Report("Please choose Data Source and Output File");
        }

        private async Task DoExportAsync(IProgress<string> progress)
        {
            progress?.Report("Running...");
            AccountReaderWithSqliteConnection accReader = new(this.DataSource, progress);
            TransactionReaderWithSqliteConnection trxReader = new(this.DataSource, progress);

            using var writer = File.CreateText(this.OutputFile);

            var runExtract = new Exporter(progress, accReader, trxReader, writer);
            await Task.Run(() => runExtract.Export());
            //await Task.Delay(1000);

            progress?.Report("GnuCashSql2Qif successfully completed.");
        }
    }
}
