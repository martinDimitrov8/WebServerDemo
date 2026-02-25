using System.Collections;
using System.Collections.Generic;

namespace BasicWebServer.Server.HTTP
{
    public class CookieCollection : IEnumerable<Cookie>
    {
        private readonly Dictionary<string, Cookie> cookies;

        public CookieCollection()
        {
            cookies = new Dictionary<string, Cookie>();
        }

        public void Add(Cookie cookie)
            => cookies[cookie.Name] = cookie;

        public bool Contains(string name)
            => cookies.ContainsKey(name);

        public Cookie this[string name]
            => cookies[name];

        public IEnumerator<Cookie> GetEnumerator()
            => cookies.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}