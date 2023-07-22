using System;
using System.ComponentModel;

namespace DirectoryListener
{
    internal class Log : INotifyPropertyChanged
    {
        public enum EventType { Created, Changed, Renamed, Deleted }
        public string Url { get; set; }
        public string longUrl { get; set; }
        public EventType FileEvent { get; set; }
        public string User { get; set; }
        public DateTime EventTime { get; set; }
        public bool isUploaded { get; set; }

        private bool isSuccessfulIcVisible;

        private bool isFailedIcVisible;

        public bool IsSuccessfulIcVisible
        {
            get { return isUploaded; }
            set
            {
                if (isUploaded != value)
                {
                    OnPropertyChanged(nameof(IsSuccessfulIcVisible));
                }
            }
        }

        public bool IsFailedIcVisible
        {
            get { return !isUploaded; }
            set
            {
                if (!isUploaded != value)
                {
                    OnPropertyChanged(nameof(IsFailedIcVisible));
                }
            }
        }

        public Log (EventType FileEvent, string url, string longUrl, string user, bool isUploaded)
        {
            this.FileEvent = FileEvent;
            this.Url = url;
            this.longUrl = longUrl;
            this.User = user;
            this.EventTime = DateTime.Now;
            this.isUploaded = isUploaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
