using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
        public Method Method { get; private set; }
        public string Url { get; private set; }
        public HeaderCollection Headers { get; private set; }
        public string Body { get; private set; }
        public Dictionary<string, string> FormData { get; private set; }

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
    }
}