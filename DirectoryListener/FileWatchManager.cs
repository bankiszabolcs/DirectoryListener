using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
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
        public static ObservableCollection<Log> logCollection = new ObservableCollection<Log>();
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
            Log actualLog = new Log(Log.EventType.Changed, e.Name, e.FullPath, Environment.UserName);
            activitiesLog.Add($"Changed: {actualLog.longUrl} by {actualLog.User} on {actualLog.EventTime}");
            string result = await SendFileAsJsonAsync(e.FullPath, API_URL);
            Console.WriteLine(result);
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

        public class FileAttachment
        {
            public string Name { get; set; }
            public string Bytes { get; set; }
        }

        public async Task<string> SendFileAsJsonAsync(string filePath, string apiUrl)
        {
            try
            {
                // Read the file data as a byte array
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Convert the byte array to a Base64-encoded string
                string fileDataAsBase64 = Convert.ToBase64String(fileBytes);

                // Create the FileAttachment instance with the file data
                FileAttachment fileAttachment = new FileAttachment
                {
                    Name = Path.GetFileName(filePath),
                    Bytes = fileDataAsBase64
                };

                // Create an HttpClient instance
                using (HttpClient httpClient = new HttpClient())
                {
                    // Set the content type header to indicate JSON data
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Serialize the FileAttachment instance to JSON
                    string jsonPayloadString = Newtonsoft.Json.JsonConvert.SerializeObject(fileAttachment);

                    // Create a StringContent object with the JSON payload
                    var content = new StringContent(jsonPayloadString, Encoding.UTF8, "application/json");

                    // Make the POST request
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        // Request failed
                        return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return $"Error: {ex.Message}";
            }
        }
        //string result = await SendFileAsJsonAsync("C:/Users/banki/OneDrive/Asztali gép/Programozás/CSHARP/torolheto/123.txt", "helo", "https://localhost:5000/files");
    }
}   
