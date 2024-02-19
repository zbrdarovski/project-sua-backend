using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace StatsHaramija.Models
{
    public class ApiCallStat
    {
        [BsonElement("Id")]
        public string Id { get; set; }
        [BsonElement("Endpoint")]
        public string Endpoint { get; set; }
        [BsonElement("Count")]
        public int Count { get; set; }
        [BsonElement("LastCalled")]
        public DateTime LastCalled { get; set; }
    }

}
