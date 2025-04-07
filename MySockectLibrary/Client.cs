using System.Net.Sockets;
using System.Text;

/// <summary>
/// Clase que representa un cliente de socket TCP.
/// </summary>
public class SocketClient
{
    // Cliente TCP utilizado para la conexión.
    private TcpClient _client = null!;

    // Flujo de red utilizado para enviar y recibir datos.
    private NetworkStream _stream = null!;

    /// <summary>
    /// Conecta el cliente al servidor especificado por la dirección IP y el puerto.
    /// </summary>
    /// <param name="ip">Dirección IP del servidor.</param>
    /// <param name="port">Puerto del servidor.</param>
    /// <returns>Una tarea que representa la operación asincrónica de conexión.</returns>
    public async Task ConnectAsync(string ip, int port)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(ip, port);
        _stream = _client.GetStream();
        Console.WriteLine($"Conectado a {ip}:{port}");
    }

    /// <summary>
    /// Envía un mensaje al servidor y espera una respuesta.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <returns>Una tarea que representa la operación asincrónica de envío.</returns>
    public async Task SendAsync(string message)
    {
        // Convertir el mensaje a un arreglo de bytes.
        byte[] data = Encoding.UTF8.GetBytes(message);

        // Enviar el mensaje al servidor.
        await _stream.WriteAsync(data, 0, data.Length);
        Console.WriteLine($"Mensaje enviado: {message}");

        // Buffer para almacenar la respuesta del servidor.
        byte[] buffer = new byte[1024];

        // Leer la respuesta del servidor.
        int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Respuesta del servidor: {response}");
    }

    /// <summary>
    /// Desconecta el cliente del servidor.
    /// </summary>
    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
    }
}
