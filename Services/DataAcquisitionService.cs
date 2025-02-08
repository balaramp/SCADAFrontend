using System.Net.Sockets;
using SCADAFrontend.Data;
using SCADAFrontend.Models;
using Microsoft.EntityFrameworkCore;
using Modbus.Device;

namespace SCADAFrontend.Services
{
    public class DataAcquisitionService
    {
        private readonly ApplicationDbContext _context;

        public DataAcquisitionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Simulate fetching real-time data from an RTU (voltage, current, breaker status)
        public async Task<RtuData> GetLatestDataAsync(string rtuIp, int port)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(rtuIp, port);
            var master = ModbusIpMaster.CreateIp(client);

            ushort voltageAddress = 100; // Example address
            ushort currentAddress = 102; // Example address

            // Read voltage and current values
            ushort[] voltageRegisters = master.ReadHoldingRegisters(1, voltageAddress, 2);
            ushort[] currentRegisters = master.ReadHoldingRegisters(1, currentAddress, 2);

            var data = new RtuData
            {
                Timestamp = DateTime.UtcNow,
                RtuId = rtuIp,
                Voltage = ConvertRegistersToFloat(voltageRegisters),
                Current = ConvertRegistersToFloat(currentRegisters),
                BreakerStatus = master.ReadCoils(1, 200, 1)[0] // Read breaker status
            };

            _context.RtuReadings.Add(data);
            //await _context.SaveChangesAsync();
            return data;
        }

        // Convert Modbus registers to float
        private float ConvertRegistersToFloat(ushort[] registers)
        {
            byte[] bytes = new byte[4];
            Buffer.BlockCopy(registers, 0, bytes, 0, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        // Fetch historical data based on start and end time
        public async Task<List<RtuData>> GetHistoricalDataAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.RtuReadings
                .Where(data => data.Timestamp >= startTime && data.Timestamp <= endTime)
                .OrderBy(data => data.Timestamp)
                .ToListAsync();
        }
    }
}

