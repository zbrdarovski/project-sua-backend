using System;

namespace StatzAPI.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string StatsCollectionName { get; set; }
    }
}
