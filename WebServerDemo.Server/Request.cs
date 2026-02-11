using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerDemo.Server
{
    public class Request
    {
        public Method Method { get; private set; }
        public string Url { get; private set; }
        public HeaderCollection Headers { get; private set; }
        public string Body { get; private set; }

        public static Request Parse(string request)
        {
            var lines = request.Split("\r\n");
            var startLine = lines[0].Split(' ');

            var method = ParseMethod(startLine[0]);
            var url = startLine[1];

            var headerLines = lines.Skip(1)
                                   .TakeWhile(l => l != string.Empty)
                                   .ToArray();

            var headers = ParseHeaders(headerLines);

            var bodyLines = lines.Skip(1 + headerLines.Length + 1);
            var body = string.Join("\r\n", bodyLines);

            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Body = body
            };
        }

        private static Method ParseMethod(string method)
        {
            if (!Enum.TryParse(method, out Method result))
            {
                throw new InvalidOperationException("Invalid HTTP method");
            }

            return result;
        }

        private static HeaderCollection ParseHeaders(string[] lines)
        {
            var headers = new HeaderCollection();

            foreach (var line in lines)
            {
                var parts = line.Split(": ");
                headers.Add(new Header(parts[0], parts[1]));
            }

            return headers;
        }
    }
}