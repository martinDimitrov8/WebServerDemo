using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;

namespace BasicWebServer.Server
{
    public class HttpServer
    {
        private readonly TcpListener listener;
        private readonly IRoutingTable routingTable;
        AddSession(request, response);
        private void AddSession(Request request, Response response)
        {
            var session = request.Session;

            if (!session.ContainsKey(Session.CurrentDateKey))
            {
                session[Session.CurrentDateKey] = DateTime.Now.ToString();
            }

            response.Cookies.Add(
                new Cookie(Session.SessionCookieName, session.Id));
        }
        public HttpServer(
            string ip,
            int port,
            Action<IRoutingTable> routes)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            routingTable = new RoutingTable();
            routes(routingTable);
        }

        public HttpServer(Action<IRoutingTable> routes)
            : this("127.0.0.1", 8080, routes)
        {
        }

        public async Task StartAsync()
        {
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                {
                    using var stream = client.GetStream();

                    var requestText = await ReadRequestAsync(stream);
                    var request = Request.Parse(requestText);

                    var response = routingTable.MatchRequest(request);

                    response.PreRenderAction?.Invoke(request, response);

                    await WriteResponseAsync(stream, response);

                    client.Close();
                });
            }
        }

        private async Task<string> ReadRequestAsync(NetworkStream stream)
        {
            var buffer = new byte[1024];
            var builder = new StringBuilder();

            int bytesRead;

            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }

        private async Task WriteResponseAsync(
     NetworkStream stream,
     Response response)
        {
            var bytes = Encoding.UTF8.GetBytes(response.ToString());
            await stream.WriteAsync(bytes);
        }
    }
}