using System.Net;
using System.Net.Sockets;
using VideothequeTcpApp.VideoService.Presentation.Helpers;

Console.WriteLine("Starting Videotheque TCP Server...");

const int PORT = 8888;

var listener = new TcpListener(IPAddress.Loopback, PORT);
listener.Start();
Console.WriteLine($"Server is listening on port {PORT}");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = Task.Run(() => ServerHelper.HandleClient(client));
}
