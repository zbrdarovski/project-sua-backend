using CommentsRatingsAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CommentsRatingsAPI
{
    public class CommentsRatingsRepository
    {
        private readonly IMongoCollection<Comment> _commentsCollection;
        private readonly IMongoCollection<Rating> _ratingsCollection;

        public CommentsRatingsRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("commentsratings");
            _commentsCollection = database.GetCollection<Comment>("comments");
            _ratingsCollection = database.GetCollection<Rating>("ratings");
        }

        // Comments Methods

        public async Task<List<Comment>> GetCommentsByItemId(string itemId)
        {
            return await _commentsCollection.Find(comment => comment.ItemId == itemId).ToListAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Id) || string.IsNullOrWhiteSpace(comment.Username) || string.IsNullOrWhiteSpace(comment.Content))
            {
                throw new ArgumentException("Invalid comment. Please provide valid data.");
            }

            comment.Timestamp = DateTime.UtcNow;
            await _commentsCollection.InsertOneAsync(comment);
        }

        public async Task<bool> EditCommentAsync(string commentId, string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
            {
                throw new ArgumentException("Invalid content. Please provide valid data.");
            }

            var filter = Builders<Comment>.Filter.Eq(comment => comment.Id, commentId);
            var update = Builders<Comment>.Update.Set(comment => comment.Content, newContent);
            var result = await _commentsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveCommentAsync(string commentId)
        {
            var result = await _commentsCollection.DeleteOneAsync(comment => comment.Id == commentId);
            return result.DeletedCount > 0;
        }

        // Ratings Methods

        public async Task<List<Rating>> GetRatingsByItemId(string itemId)
        {
            return await _ratingsCollection.Find(rating => rating.ItemId == itemId).ToListAsync();
        }


        public async Task AddRatingAsync(Rating rating)
        {
            if (string.IsNullOrWhiteSpace(rating.Id) || string.IsNullOrWhiteSpace(rating.Username) || rating.Value < 1 || rating.Value > 5)
            {
                throw new ArgumentException("Invalid rating. Please provide valid data.");
            }

            rating.Timestamp = DateTime.UtcNow;
            await _ratingsCollection.InsertOneAsync(rating);
        }

        public async Task<bool> RemoveRatingAsync(string ratingId)
        {
            var result = await _ratingsCollection.DeleteOneAsync(rating => rating.Id == ratingId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> EditRatingAsync(string ratingId, int newValue)
        {
            if (newValue < 1 || newValue > 5)
            {
                throw new ArgumentException("Invalid rating value. Please provide a value between 1 and 5.");
            }

            var filter = Builders<Rating>.Filter.Eq(rating => rating.Id, ratingId);
            var update = Builders<Rating>.Update.Set(rating => rating.Value, newValue);
            var result = await _ratingsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

    }
}
