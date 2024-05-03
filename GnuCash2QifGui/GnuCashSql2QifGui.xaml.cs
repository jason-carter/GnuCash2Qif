using GnuCash.Sql2Qif.Library;
using GnuCash.Sql2Qif.Library.DAL.Readers;
using GnuCash.Sql2Qif.Library.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace GnuCash2QifGui
{
    /// <summary>
    /// Interaction logic for GnuCashSql2QifGui.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataSourceText.Text.Trim().Equals(String.Empty))
            {
                MessageBox.Show("You must select a GNU Cash SQL file to export!",
                                "No file selected!",
                                MessageBoxButton.OK);
                return;
            }
            if (OutputFileText.Text.Trim().Equals(String.Empty))
            {
                MessageBox.Show("You must select an output file to export to!",
                                "No file selected!",
                                MessageBoxButton.OK);
                return;
            }

            // TODO: check the output file doesn't exist, confirm overwrite if it does
            // TODO: check if the dataource is a valid GnuCash Sqlite file

            var progress = new Progress<int>(value => Progress.Value = value);
            var conversion = new Progress<string>(value => ConvertCount.Content = value);
            var status = new Progress<string>(value => StatusReport.Content = value);
            var dataSourcePath = System.IO.Path.GetFullPath(DataSourceText.Text);
            var outputPath = System.IO.Path.GetFullPath(OutputFileText.Text);

            await Task.Run(() => DoExport(dataSourcePath, outputPath, progress, conversion, status));
        }

        private static async Task DoExport(string dataSourcePath, string outputPath, IProgress<int> progress, IProgress<string> conversion, IProgress<string> status)
        {
            //var runExtract = new Extractor();
            //runExtract.LogEvent += HandleLogEvent;
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddDebug();
            });
            // var gnuCashLogger = loggerFactory.CreateLogger<GnuCashSql2QifGui>();

            status.Report($"Running...");
            AccountReaderWithSqliteConnection accReader = new(dataSourcePath, loggerFactory.CreateLogger<Account>());
            TransactionReaderWithSqliteConnection trxReader = new(dataSourcePath, loggerFactory.CreateLogger<Transaction>());

            using (var writer = File.CreateText(outputPath))
            {
                var runExtract = new Exporter(loggerFactory.CreateLogger<Account>(), accReader, trxReader, writer);
                await Task.Run(() => runExtract.Export());
            }

            //await Task.Run(() => runExtract.ExtractData(dataSourcePath, outputPath));
            status.Report("GnuCashSql2Qif successfully completed.");
        }

        private void DataSourceSelectButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".gnucash";
            dlg.Filter = "GNU Cash Files (*.gnucash)|*.gnucash|SQLite Files (*.sqlite)|*.sqlite";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value == true)
            {
                DataSourceText.Text = dlg.FileName;
            }
        }

        private void OutputFileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".qif";
            dlg.Filter = "Quicken Interchange Format Files (*.qif)|*.qif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value == true)
            {
                OutputFileText.Text = dlg.FileName;
            }
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
