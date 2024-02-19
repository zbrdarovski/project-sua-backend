namespace StatzAPI.Models
{
    public class StatsData
    {
        public string Endpoint { get; set; }
        public int CallCount { get; set; }
        public DateTime LastAccessed { get; internal set; }
    }
}
