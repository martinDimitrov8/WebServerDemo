using System;
using System.Collections.Generic;

namespace BasicWebServer.Server.HTTP
{
    public class Session
    {
        public const string SessionCookieName = "SessionId";
        public const string CurrentDateKey = "CurrentDate";
        public const string UserKey = "User";

        private readonly Dictionary<string, string> data;

        public Session(string id)
        {
            Id = id;
            data = new Dictionary<string, string>();
        }

        public string Id { get; }

        public string this[string key]
        {
            get => data[key];
            set => data[key] = value;
        }

        public bool ContainsKey(string key)
            => data.ContainsKey(key);

        public void Clear()
            => data.Clear();
    }
}