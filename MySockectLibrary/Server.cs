using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace MySockectLibrary
{
    
    public class SocketServer
    {
        private TcpListener _listener;
        private bool _isRunning;
        private readonly int _maxRetries;
        private readonly int _retryDelayMs;

        public SocketServer(int port, int maxRetries = 3, int retryDelayMs = 1000)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _maxRetries = maxRetries;
            _retryDelayMs = retryDelayMs;
        }

        public async Task StartAsync()
        {
            _isRunning = true;
            _listener.Start();
            Console.WriteLine($"Server iniciado en puerto {((IPEndPoint)_listener.LocalEndpoint).Port}");

            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientWithRetryAsync(client);
                }
                catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException)
                {
                    if (!_isRunning) return; // Detención normal del servidor

                    Console.WriteLine($"Error aceptando conexión: {ex.Message}");
                    await Task.Delay(_retryDelayMs);
                }
            }
        }

        private async Task HandleClientWithRetryAsync(TcpClient client)
        {
            int retryCount = 0;

            while (retryCount < _maxRetries)
            {
                try
                {
                    await HandleClientAsync(client);
                    return;
                }
                catch (Exception ex) when (ex is SocketException || ex is IOException)
                {
                    retryCount++;
                    Console.WriteLine($"Intento {retryCount} de manejo de cliente fallido: {ex.Message}");

                    if (retryCount >= _maxRetries)
                    {
                        Console.WriteLine($"No se pudo manejar el cliente después de {_maxRetries} intentos");
                        return;
                    }

                    await Task.Delay(_retryDelayMs);
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (var stream = client.GetStream())
                {
                    while (true) // Mantener la conexión abierta para múltiples mensajes
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead == 0) // Cliente cerró la conexión
                            break;

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Mensaje recibido: {message}");

                        string response = $"{message}";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                }
            }
            finally
            {
                client.Dispose();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }
    }
}
