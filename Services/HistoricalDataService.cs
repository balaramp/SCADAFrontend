using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCADAFrontend.Data;
using SCADAFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace SCADAFrontend.Services
{
    public class HistoricalDataService
    {
        private readonly ApplicationDbContext _context;

        public HistoricalDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to store historical data in the SQL database
        public async Task StoreHistoricalDataAsync(List<RtuData> retrievedData)
        {
            var historicalRecords = new List<HistoricalData>();

            foreach (var data in retrievedData)
            {
                historicalRecords.Add(new HistoricalData
                {
                    Timestamp = data.Timestamp,
                    RtuId = data.RtuId,
                    Voltage = data.Voltage,
                    Current = data.Current
                });
            }

            await _context.HistoricalReadings.AddRangeAsync(historicalRecords);
            await _context.SaveChangesAsync();
        }

        // Retrieve historical data from the database
        public async Task<List<HistoricalData>> GetHistoricalDataAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.HistoricalReadings
                .Where(d => d.Timestamp >= startTime && d.Timestamp <= endTime)
                .OrderBy(d => d.Timestamp)
                .ToListAsync();
        }
    }
}
