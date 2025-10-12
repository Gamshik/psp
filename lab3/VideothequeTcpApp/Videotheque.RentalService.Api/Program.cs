using System.Net;
using System.Net.Sockets;
using Videotheque.RentalService.Api.Presentation.Helpers;

const int servicePort = 9003; 
var listener = new TcpListener(IPAddress.Any, servicePort);
listener.Start();
Console.WriteLine($"RentalService listening on port {servicePort}");

while (true)
{
    var client = listener.AcceptTcpClient();
    _ = Task.Run(() => ServerHelper.HandleClientAsync(client));
}