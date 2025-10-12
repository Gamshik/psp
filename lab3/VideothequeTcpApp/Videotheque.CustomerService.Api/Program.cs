using System.Net;
using System.Net.Sockets;
using Videotheque.CustomerService.Api.Presentation.Helpers;

const int servicePort = 9002; 
var listener = new TcpListener(IPAddress.Any, servicePort);
listener.Start();
Console.WriteLine($"CustomerService listening on port {servicePort}");

while (true)
{
    var client = listener.AcceptTcpClient();
    _ = Task.Run(() => ServerHelper.HandleClientAsync(client));
}