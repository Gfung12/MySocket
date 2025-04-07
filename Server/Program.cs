using MySockectLibrary;
using System.Net;

class Program
{
    static async Task Main(string[] args)
    {
        var server = new SocketServer(8080); // Puerto 8080
        Console.WriteLine("Iniciando servidor... (Presiona Ctrl+C para detener)");

        var serverTask = server.StartAsync();

        // Mantener el servidor corriendo hasta que se presione una tecla
        Console.ReadKey();
        server.Stop();
        Console.WriteLine("Servidor detenido");
    }
}