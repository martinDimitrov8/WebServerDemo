using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServerDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ipAdrress = IPAddress.Parse("127.0.0.1");
            int port = 8080;

            var serverListener = new TcpListener(ipAdrress,port);
            serverListener.Start();

            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine("Listening for request...");
            var connection = serverListener.AcceptTcpClient();
            var networkStream = connection.GetStream();

            var content = "Hello from my web server";
            var contentLength = Encoding.UTF8.GetByteCount(content);

            var response = $@"HTTP/1.1 200 OK
Content-Type: text/plain
Conetnt-Length: {contentLength}

{content}";

            byte[] responseBytes = Encoding.UTF8.GetBytes(content);
            networkStream.Write(responseBytes);
            connection.Close();
        }
    }
}
