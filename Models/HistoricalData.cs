using System;

namespace SCADAFrontend.Models
{
    public class HistoricalData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string RtuId { get; set; } = string.Empty;
        public float Voltage { get; set; }
        public float Current { get; set; }
    }
}

