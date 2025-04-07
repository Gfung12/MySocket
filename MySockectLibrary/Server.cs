using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace MySockectLibrary
{
    /// <summary>
    /// Clase que representa un servidor de socket TCP.
    /// </summary>
    public class SocketServer
    {
        // Listener TCP utilizado para aceptar conexiones de clientes.
        private TcpListener _listener;

        // Indica si el servidor está en ejecución.
        private bool _isRunning;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SocketServer"/> con el puerto especificado.
        /// </summary>
        /// <param name="port">Puerto en el que el servidor escuchará las conexiones entrantes.</param>
        public SocketServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// Inicia el servidor y comienza a aceptar conexiones de clientes.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica de inicio del servidor.</returns>
        public async Task StartAsync()
        {
            _isRunning = true;
            _listener.Start();
            Console.WriteLine($"Server iniciado en puerto {((IPEndPoint)_listener.LocalEndpoint).Port}");

            while (_isRunning)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client); // Manejar cliente en segundo plano
            }
        }

        /// <summary>
        /// Maneja la comunicación con un cliente conectado.
        /// </summary>
        /// <param name="client">Cliente TCP conectado.</param>
        /// <returns>Una tarea que representa la operación asincrónica de manejo del cliente.</returns>
        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Mensaje recibido: {message}");

                // Respuesta automática
                string response = "Mensaje recibido por el servidor";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }

        /// <summary>
        /// Detiene el servidor y deja de aceptar conexiones de clientes.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }
    }
}
