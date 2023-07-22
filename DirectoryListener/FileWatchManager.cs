using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Application;


namespace DirectoryListener
{
    internal class FileWatchManager
    {
        public FileSystemWatcher watcher;
        public static ObservableCollection<Log> logCollection = new ObservableCollection<Log>(); //{new Log(Log.EventType.Changed, "as.txt", "sdfdsfdsf", "Szabi", true), new Log(Log.EventType.Changed, "as.txt", "sdfdsfdsf", "Szabi", false) , new Log(Log.EventType.Changed, "as.txt", "sdfdsfdsf", "Szabi", false) };
        public static List<string> activitiesLog = new List<string>();
        string API_URL = "https://localhost:5000/files";

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
        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
            string result = await SendFileAsJsonAsync(e.FullPath, e.Name, API_URL);
            Console.WriteLine(result);
            Log actualLog = new Log(Log.EventType.Changed, e.Name, e.FullPath, Environment.UserName, !result.StartsWith("Error:"));
            activitiesLog.Add($"Changed: {actualLog.longUrl} by {actualLog.User} on {actualLog.EventTime}. Uploaded to a server: {(actualLog.isUploaded? "Yes":"No") }");
            Application.Current.Dispatcher.Invoke(() =>
            {
               logCollection.Add(actualLog);
            });


        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            string result = await SendFileAsJsonAsync(e.FullPath, e.Name, API_URL);
            Console.WriteLine(result);
            Log actualLog = new Log(Log.EventType.Created, e.Name, e.FullPath, Environment.UserName, !result.StartsWith("Error:"));
            activitiesLog.Add($"Created: {actualLog.longUrl} by {actualLog.User} on {actualLog.EventTime.ToString()} Uploaded to a server: {(actualLog.isUploaded ? "Yes" : "No")}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                logCollection.Add(actualLog);
            });
        }

        private async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");
            Log actualLog = new Log(Log.EventType.Deleted, e.Name, e.FullPath, Environment.UserName, false);
            activitiesLog.Add($"Deleted: {actualLog.longUrl} by {actualLog.User} on {actualLog.EventTime} Uploaded to a server: {(actualLog.isUploaded ? "Yes" : "No")}");
            Application.Current.Dispatcher.Invoke(() => { logCollection.Add(actualLog); });
        }

        private async void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
            string result = await SendFileAsJsonAsync(e.FullPath, e.Name, API_URL);
            Console.WriteLine(result);
            Log actualLog = new Log(Log.EventType.Renamed, e.Name, e.FullPath, Environment.UserName, !result.StartsWith("Error:"));
            activitiesLog.Add($"Renamed: {e.OldFullPath} was renamed to {e.FullPath} by {actualLog.User} on {actualLog.EventTime} Uploaded to a server: {(actualLog.isUploaded ? "Yes" : "No")}");
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
                watcher = null;
            }

        }

        public class FileAttachment
        {
            public string Name { get; set; }
            public string Bytes { get; set; }
        }

        public async Task<string> SendFileAsJsonAsync(string filePath, string name, string apiUrl)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);

                string fileDataAsBase64 = Convert.ToBase64String(fileBytes);

                FileAttachment fileAttachment = new FileAttachment
                {
                    Name = name,
                    Bytes = fileDataAsBase64
                };

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string jsonPayloadString = Newtonsoft.Json.JsonConvert.SerializeObject(fileAttachment);

                    var content = new StringContent(jsonPayloadString, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}   
