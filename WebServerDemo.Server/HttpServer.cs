using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServerDemo.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener listener;

        public HttpServer(string ipAddress, int port)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;
            this.listener = new TcpListener(this.ipAddress, this.port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine($"Server started at http://localhost:{port}");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                using var networkStream = client.GetStream();

                var requestText = ReadRequest(networkStream);
                Console.WriteLine(requestText);

                WriteResponse(networkStream, "Hello from the server!");
                client.Close();
            }
        }

        private string ReadRequest(NetworkStream networkStream)
        {
            var buffer = new byte[1024];
            var requestBuilder = new StringBuilder();
            int totalBytes = 0;

            do
            {
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                totalBytes += bytesRead;

                if (totalBytes > 10_000)
                {
                    throw new InvalidOperationException("Request too large");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
            while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }

        private void WriteResponse(NetworkStream networkStream, string message)
        {
            var body = message;
            var bodyBytes = Encoding.UTF8.GetBytes(body);

            var response =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain; charset=UTF-8\r\n" +
                $"Content-Length: {bodyBytes.Length}\r\n" +
                "\r\n" +
                body;

            var responseBytes = Encoding.UTF8.GetBytes(response);
            networkStream.Write(responseBytes);
        }
    }
}