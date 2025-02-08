namespace SCADAFrontend.Models
{
    public class RtuData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string RtuId { get; set; } = string.Empty;
        public float Voltage { get; set; }
        public float Current { get; set; }
        public bool BreakerStatus { get; set; }
    }
}
