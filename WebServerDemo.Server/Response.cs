using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerDemo.Server
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            StatusCode = statusCode;
            Headers = new HeaderCollection();

            Headers.Add(new Header("Server", "My Web Server"));
            Headers.Add(new Header("Content-Type", "text/plain; charset=UTF-8"));
        }

        public StatusCode StatusCode { get; }
        public HeaderCollection Headers { get; }
        public string Body { get; set; }
    }
}