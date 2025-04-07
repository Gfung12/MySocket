using MySockectLibrary;
using System.Net;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new SocketClient();

        try
        {
            Console.WriteLine("Conectando al servidor...");
            await client.ConnectAsync("127.0.0.1", 8080);

            while (true)
            {
                Console.Write("Escribe un mensaje (o 'salir' para terminar): ");
                var message = Console.ReadLine();

                if (message?.ToLower() == "salir")
                    break;

                await client.SendAsync(message!);
            }
        }
        finally
        {
            client.Disconnect();
            Console.WriteLine("Desconectado");
        }
    }
}