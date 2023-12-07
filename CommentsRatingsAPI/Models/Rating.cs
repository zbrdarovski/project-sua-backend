using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CommentsRatingsAPI.Models
{
    public class Rating
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("itemId")]
        public string ItemId { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Value")]
        public int Value { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
