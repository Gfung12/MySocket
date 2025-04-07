using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Clase que representa un cliente de socket TCP.
/// </summary>
public class SocketClient
{
    private TcpClient _client = null!;
    private NetworkStream _stream = null!;
    private readonly int _maxRetries;
    private readonly int _retryDelayMs;

    public SocketClient(int maxRetries = 3, int retryDelayMs = 1000)
    {
        _maxRetries = maxRetries;
        _retryDelayMs = retryDelayMs;
    }

    public async Task ConnectAsync(string ip, int port)
    {
        int retryCount = 0;
        while (retryCount < _maxRetries)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();
                Console.WriteLine($"Conectado a {ip}:{port}");
                return;
            }
            catch (Exception ex) when (ex is SocketException || ex is IOException)
            {
                retryCount++;
                Console.WriteLine($"Intento {retryCount} de conexión fallido: {ex.Message}");

                if (retryCount >= _maxRetries)
                    throw new Exception($"No se pudo conectar después de {_maxRetries} intentos", ex);

                await Task.Delay(_retryDelayMs);
            }
        }
    }

    public async Task SendAsync(string message)
    {
        int retryCount = 0;
        byte[] data = Encoding.UTF8.GetBytes(message);

        while (retryCount < _maxRetries)
        {
            try
            {
                
                await _stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine($"Mensaje enviado: {message}");

                byte[] buffer = new byte[1024];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Respuesta del servidor: {response}");
                return;
            }
            catch (Exception ex) when (ex is SocketException || ex is IOException)
            {
                retryCount++;
                Console.WriteLine($"Intento {retryCount} de envío fallido: {ex.Message}");

                if (retryCount >= _maxRetries)
                    throw new Exception($"No se pudo enviar el mensaje después de {_maxRetries} intentos", ex);

                await Task.Delay(_retryDelayMs);

                // Intentar reconectar antes del próximo reintento
                await TryReconnectAsync();
            }
        }
    }

    private async Task TryReconnectAsync()
    {
        try
        {
            if (_client?.Connected == false)
            {
                Console.WriteLine("Intentando reconectar...");
                await ConnectAsync(((IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString(),
                                 ((IPEndPoint)_client.Client.RemoteEndPoint).Port);
            }
        }
        catch
        {
            // Silenciar errores de reconexión, ya que el reintento principal manejará esto
        }
    }

    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
    }
}

