using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace DirectoryListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> extensions = new List<string>();
        string dirPath;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void WatchFile(string url, string format)
        {
            var watcher = new FileSystemWatcher(url);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = $"*{format}";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description =
            "Válaszd ki azt a mappát, ahol meg szeretnél figyelni egy fájlt.";

            folderBrowserDialog1.ShowNewFolderButton = false;

            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.

            if (result == System.Windows.Forms.DialogResult.OK) // Test result.
            {
                dirPath = folderBrowserDialog1.SelectedPath;

                fileURL.Text = dirPath;
            }
        }

        private void cbAllCheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newVal = (allExt.IsChecked == true);
            txtExt.IsChecked = newVal;
            jpgExt.IsChecked = newVal;
            docExt.IsChecked = newVal;
            pdfExt.IsChecked = newVal;
        }

        private void cbSingleCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (txtExt.IsChecked == true && jpgExt.IsChecked == true && docExt.IsChecked == true && pdfExt.IsChecked == true)
            {
                allExt.IsChecked = true;
            }
            if (txtExt.IsChecked == false && jpgExt.IsChecked == false && docExt.IsChecked == false && pdfExt.IsChecked == true)
            {
                allExt.IsChecked = false;
            }

            string actualExt = (string)((CheckBox)sender).Content;
            Console.WriteLine(actualExt);
            if (extensions.Contains(actualExt))
            {
                extensions.Remove(actualExt);
            }
            else
            {
                extensions.Add(actualExt);
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }

        private void StartMonitor(object sender, RoutedEventArgs e)
        {
             foreach (var ext in extensions)
             {
                WatchFile(dirPath, ext);
            }
            //Gomb legyen disabled ha megy a figyelés
        }

        private void StopMonitor(object sender, RoutedEventArgs e)
        {
            //gomb alapértelmezett figyelés leáll
        }

        private void Logger()
        {

        }


    }
}
