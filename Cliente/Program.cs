using MySockectLibrary;
using System.Net;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new SocketClient(maxRetries: 3, retryDelayMs: 1000);

        try
        {
            await client.ConnectAsync("127.0.0.1", 8080);

            while (true)
            {
                Console.Write("Ingrese mensaje (o 'salir' para terminar): ");
                var message = Console.ReadLine();

                if (message?.ToLower() == "salir")
                    break;

                try
                {
                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error grave: {ex.Message}");
                    Console.WriteLine("¿Reconectar? (s/n)");
                    if (Console.ReadLine()?.ToLower() == "s")
                    {
                        await client.ConnectAsync("127.0.0.1", 8080);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        finally
        {
            client.Disconnect();
            Console.WriteLine("Desconectado del servidor.");
        }
    }
}