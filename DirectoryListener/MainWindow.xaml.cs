using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DirectoryListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileWatchManager fileWatcher;
        List<string> extensions = new List<string>();
        List<CheckBox> checkBoxes = new List<CheckBox>();
        string dirPath;

        public MainWindow()
        {
            InitializeComponent();
            LoadCheckBoxes();
            var logCollection = FileWatchManager.logCollection;
            logContainer.ItemsSource = logCollection;
            logCollection.CollectionChanged += LogCollection_CollectionChanged;
            fileWatcher = new FileWatchManager();
        }

        private void LogCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Scroll to the last item when a new log is added
                logContainer.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //BrowseDir();
            dirPath = BrowseDir("Válaszd ki azt a mappát, ahol meg szeretnél figyelni egy fájlt.");
            fileURL.Text = dirPath;
        }

        private string BrowseDir(string description)
        {
            FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description = description;

            folderBrowserDialog1.ShowNewFolderButton = false;

            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.

            if (result == System.Windows.Forms.DialogResult.OK) // Test result.
            {
                return folderBrowserDialog1.SelectedPath;

                //fileURL.Text = dirPath;
            }
            return null;
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

        private void StartMonitor(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(dirPath))
            {
                foreach (var ext in extensions)
                {
                    fileWatcher.WatchFile(dirPath, ext);
                }
           
                Spinner.Visibility = Visibility.Visible;
                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
            
            }
            else
            {
                MessageBox.Show("Válassz ki egy mappát, ahol meg akarod figyelni a fájlokat", "Nincs mappa kijelölve", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BrowseDir("Válaszd ki azt a mappát, ahol meg szeretnél figyelni egy fájlt.");
            }
        }

        private void StopMonitor(object sender, RoutedEventArgs e)
        {
            Spinner.Visibility = Visibility.Hidden;
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            //FileWatchManager.watcher.EnableRaisingEvents = false;
       
            fileWatcher.StopMonitor();
            fileWatcher = null; // Set it to null to indicate it's no longer active
            
        }

        private void SaveLog_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string urlToSave = BrowseDir("Válaszd ki hova szeretnéd menteni a naplófájlt");
                File.WriteAllLines($"{urlToSave}\\activities.txt", FileWatchManager.activitiesLog);
                MessageBox.Show("Sikeres mentés", "Naplózás", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  
        }
    }
}
