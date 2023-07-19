//using Microsoft.Win32;
using System;
//using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
//using System.Diagnostics.Eventing.Reader;
using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading;
//using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Forms;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using Application = System.Windows.Application;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.Forms.MessageBox;
//using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace DirectoryListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> extensions = new List<string>();
        List<CheckBox> checkBoxes = new List<CheckBox>();
        static ObservableCollection<Log> logCollection = new ObservableCollection<Log>();
        string dirPath;
        bool isObserving = false;
        FileSystemWatcher watcher;

        public MainWindow()
        {
            InitializeComponent();
            LoadCheckBoxes();
            logContainer.ItemsSource = logCollection;
            logCollection.CollectionChanged += LogCollection_CollectionChanged;
        }

        private void LogCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Scroll to the last item when a new log is added
                logContainer.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
            }
        }

        private void WatchFile(string url, string format)
        {
            watcher = new FileSystemWatcher(url);

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
            isObserving = true;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BrowseDir();
        }

        private void BrowseDir()
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

        private void LoadCheckBoxes()
        {
            checkBoxes.Add(txtExt);
            checkBoxes.Add(jpegExt);
            checkBoxes.Add(jpgExt);
            checkBoxes.Add(pngExt);
            checkBoxes.Add(pdfExt);
            checkBoxes.Add(docExt);
            checkBoxes.Add(docXExt);
            checkBoxes.Add(pptExt);
            checkBoxes.Add(pptXExt);
            checkBoxes.Add(htmExt);
            checkBoxes.Add(htmlExt);
            checkBoxes.Add(xlsxExt);
            checkBoxes.Add(xlsExt);
        }

        private void cbAllCheckedChanged(object sender, RoutedEventArgs e)
        {
            bool newVal = (allExt.IsChecked == true);

            foreach (var cb in checkBoxes)
            {
                cb.IsChecked = newVal;
            }
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
            Log actualLog = new Log(Log.EventType.Changed, e.Name, Environment.UserName);

            Application.Current.Dispatcher.Invoke(() =>
            {
                logCollection.Add(actualLog);
            });

            
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Log actualLog = new Log(Log.EventType.Created, e.Name, Environment.UserName);
            Application.Current.Dispatcher.Invoke(() =>
            {
                logCollection.Add(actualLog);
            });
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e){
            Console.WriteLine($"Deleted: {e.FullPath}");
            Log actualLog = new Log(Log.EventType.Deleted, e.Name, Environment.UserName);
            Application.Current.Dispatcher.Invoke(() => { logCollection.Add(actualLog); });
       }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
            Log actualLog = new Log(Log.EventType.Renamed, $"{e.OldName} -> {e.Name}", Environment.UserName);
            Application.Current.Dispatcher.Invoke(() => { logCollection.Add(actualLog); });
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
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                PrintException(ex.InnerException);
            }
        }

        private void StartMonitor(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(dirPath))
            { 
                foreach (var ext in extensions)
                {
                WatchFile(dirPath, ext);
                }
                if (isObserving)
                {
                    Spinner.Visibility = Visibility.Visible;
                    startButton.IsEnabled = false;
                    stopButton.IsEnabled = true;
                    //Csak akkor ha sikeres.
                }
            }
            else
            {
                MessageBox.Show("Válassz ki egy mappát, ahol meg akarod figyelni a fájlokat", "Nincs mappa kijelölve", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BrowseDir();
            }
        }

        private void StopMonitor(object sender, RoutedEventArgs e)
        {
            isObserving = false;
            Spinner.Visibility = Visibility.Hidden;
            startButton.IsEnabled = true;
            watcher.EnableRaisingEvents = false;
        }

        private void Logger()
        {

        }
    }
}
