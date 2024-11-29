using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Windows;

namespace GnuCash2QifGui
{
    /// <summary>
    /// Interaction logic for GnuCashSql2QifGui.xaml
    /// </summary>
    public partial class MainWindow : Window, ICloseable
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        }

        public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        private void DataSourceSelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".sqlite",
                Filter = "GNU Cash Files (*.gnucash)|*.gnucash|SQLite Files (*.sqlite)|*.sqlite"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value == true)
            {
                DataSourceText.Text = dlg.FileName;
            }
        }

        private void OutputFileSelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".qif",
                Filter = "Quicken Interchange Format Files (*.qif)|*.qif"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value == true)
            {
                OutputFileText.Text = dlg.FileName;
            }
        }

        private void CloseWindow_OnClick(object sender, RoutedEventArgs e) => this.Close();
    }
}
