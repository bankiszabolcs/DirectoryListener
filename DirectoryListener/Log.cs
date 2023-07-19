using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryListener
{
    internal class Log
    {
        public enum EventType { Created, Changed, Renamed, Deleted }
        public string Url { get; set; }
        public string longUrl { get; set; }
        public EventType FileEvent { get; set; }
        public string User { get; set; }
        public DateTime EventTime { get; set; }

        public Log (EventType FileEvent, string url, string longUrl, string user)
        {
            this.FileEvent = FileEvent;
            this.Url = url;
            this.longUrl = longUrl;
            this.User = user;
            this.EventTime = DateTime.Now;
        }
    }
}
