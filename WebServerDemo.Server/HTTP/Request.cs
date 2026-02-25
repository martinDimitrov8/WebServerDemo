using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServerDemo.Server;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
        public Method Method { get; private set; }
        public string Url { get; private set; }
        public HeaderCollection Headers { get; private set; }
        public string Body { get; private set; }
        public Dictionary<string, string> FormData { get; private set; }
        public CookieCollection Cookies { get; private set; }
        public Session Session { get; private set; }

        private static readonly Dictionary<string, Session> sessions
            = new Dictionary<string, Session>();
        public static Request Parse(string requestText)
        {
            var lines = requestText.Split("\r\n");
            var startLine = lines[0].Split(' ');

            var method = Enum.Parse<Method>(startLine[0]);
            var url = startLine[1];

            var headerLines = lines.Skip(1)
                                   .TakeWhile(l => l != string.Empty)
                                   .ToArray();

            var headers = new HeaderCollection();
            var cookies = ParseCookies(headers);
            Cookies = cookies,

                var session = GetSession(cookies);
            Session = session,

            foreach (var line in headerLines)
            {
                var parts = line.Split(": ");
                headers.Add(new Header(parts[0], parts[1]));
            }

            var body = string.Join("\r\n",
                lines.Skip(1 + headerLines.Length + 1));

            var formData = ParseForm(headers, body);

            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Body = body,
                FormData = formData
            };
        }
        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookies = new CookieCollection();

            if (!headers.Contains(Header.Cookie))
                return cookies;

            var cookieHeader = headers[Header.Cookie].Value;
            var cookiePairs = cookieHeader.Split("; ");

            foreach (var pair in cookiePairs)
            {
                var parts = pair.Split('=');
                cookies.Add(new Cookie(parts[0], parts[1]));
            }

            return cookies;
        }
        private static Dictionary<string, string> ParseForm(
            HeaderCollection headers, string body)
        {
            var result = new Dictionary<string, string>();

            if (headers.Contains(Header.ContentType) &&
                headers[Header.ContentType].Value == ContentType.Form)
            {
                var pairs = body.Split('&');

                foreach (var pair in pairs)
                {
                    var parts = pair.Split('=');

                    var key = HttpUtility.UrlDecode(parts[0]);
                    var value = HttpUtility.UrlDecode(parts[1]);

                    result[key] = value;
                }
            }

            return result;
        }
        private static Session GetSession(CookieCollection cookies)
        {
            string sessionId;

            if (cookies.Contains(Session.SessionCookieName))
            {
                sessionId = cookies[Session.SessionCookieName].Value;
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
            }

            if (!sessions.ContainsKey(sessionId))
            {
                sessions[sessionId] = new Session(sessionId);
            }

            return sessions[sessionId];
        }
    }
}