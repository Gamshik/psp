using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HttpsServer;

internal class Program
{
    public static void Main(string[] args)
    {
        string certificatePath = "../../certificate.pfx";
        string certificatePassword = "gleb";

        if (!File.Exists(certificatePath))
        {
            Console.WriteLine($"Ошибка: Сертификат не найден по пути '{certificatePath}'.");
            return;
        }

        HttpsServer server = new("127.0.0.1", 443, certificatePath, certificatePassword);

        try
        {
            server.Start();
            Task.Delay(Timeout.Infinite).Wait();
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Ошибка сокета: {e.Message}.");
        }
    }
}