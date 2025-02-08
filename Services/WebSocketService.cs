using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using Modbus.Device;

namespace SCADAFrontend.Services
{
    public class WebSocketService
    {
        private readonly ConcurrentDictionary<WebSocket, Task> _clients = new();

        public async Task HandleWebSocketConnection(WebSocket socket, string rtuIp, int port)
        {
            _clients.TryAdd(socket, Task.CompletedTask);

            try
            {
                using (var client = new TcpClient(rtuIp, port))
                {
                    var master = ModbusIpMaster.CreateIp(client);

                    while (socket.State == WebSocketState.Open)
                    {
                        // Read voltage & current registers from RTU
                        ushort[] voltageRegisters = master.ReadHoldingRegisters(1, 100, 2);
                        ushort[] currentRegisters = master.ReadHoldingRegisters(1, 102, 2);

                        // Format message
                        string message = $"Voltage: {voltageRegisters[0]}, Current: {currentRegisters[0]}";
                        byte[] buffer = Encoding.UTF8.GetBytes(message);

                        // Send real-time data to WebSocket client
                        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        await Task.Delay(5000); // Send data every 5 seconds
                    }
                }
            }
            catch (Exception)
            {
                _clients.TryRemove(socket, out _);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }
}
