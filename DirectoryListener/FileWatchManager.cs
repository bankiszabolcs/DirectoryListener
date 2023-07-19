using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DirectoryListener
{
    internal class FileWatchManager
    {
        public FileSystemWatcher watcher;
        public static ObservableCollection<Log> logCollection = new ObservableCollection<Log>();
        public static List<string> activitiesLog = new List<string>();
        public void WatchFile(string url, string format)
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
        }
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
            Log actualLog = new Log(Log.EventType.Changed, e.Name, e.FullPath, Environment.UserName);
            activitiesLog.Add($"Changed: {actualLog.longUrl} by {actualLog.User} on {actualLog.EventTime}");

            Application.Current.Dispatcher.Invoke(() =>
            {
               logCollection.Add(actualLog);
            });


        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Log actualLog = new Log(Log.EventType.Created, e.Name, e.FullPath, Environment.UserName);
            activitiesLog.Add($"Created: {actualLog.longUrl} by {actualLog.User} on {actualLog.EventTime.ToString()}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                logCollection.Add(actualLog);
            });
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");
            Log actualLog = new Log(Log.EventType.Deleted, e.Name, e.FullPath, Environment.UserName);
            activitiesLog.Add($"Deleted: {actualLog.longUrl} by {actualLog.User} on {actualLog.EventTime}");
            Application.Current.Dispatcher.Invoke(() => { logCollection.Add(actualLog); });
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
            Log actualLog = new Log(Log.EventType.Renamed, e.Name, e.FullPath, Environment.UserName);
            activitiesLog.Add($"Renamed: {e.OldFullPath} was renamed to {e.FullPath} by {actualLog.User} on {actualLog.EventTime}");
            Application.Current.Dispatcher.Invoke(() => { logCollection.Add(actualLog); });
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                PrintException(ex.InnerException);
            }
        }

        public void StopMonitor()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null; // Set it to null to indicate it's no longer active
            }

        }
    }
}
