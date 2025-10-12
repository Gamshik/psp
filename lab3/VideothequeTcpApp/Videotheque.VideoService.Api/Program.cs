using System.Net;
using System.Net.Sockets;
using Videotheque.VideoService.Api.Presentation.Helpers;

const int servicePort = 9001;
var listener = new TcpListener(IPAddress.Any, servicePort);
listener.Start();
Console.WriteLine($"VideoService listening on port {servicePort}");

while (true)
{
    var client = listener.AcceptTcpClient();
    _ = Task.Run(() => ServerHelper.HandleClientAsync(client));
}