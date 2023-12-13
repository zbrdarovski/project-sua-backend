using CommentsRatingsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

namespace CommentsRatingsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentsRatingsController : ControllerBase
    {
   
        private readonly ILogger<CommentsRatingsController> _logger;
        private readonly CommentsRatingsRepository _commentsRatingsRepository;

        public CommentsRatingsController(ILogger<CommentsRatingsController> logger, CommentsRatingsRepository commentsRatingsRepository)
        {
            _logger = logger;
            _commentsRatingsRepository = commentsRatingsRepository;
        }

        // Comments Endpoints

        [HttpGet("comments/{itemId}", Name = "GetCommentsByItemId")]
        public async Task<IEnumerable<Comment>> GetCommentsByItemId(string itemId)
        {
            var comments = await _commentsRatingsRepository.GetCommentsByItemId(itemId);
            return comments;
        }


        [HttpPost("comments", Name = "AddComment")]
        public async Task<IActionResult> AddCommentAsync([FromBody] Comment comment)
        {
            try
            {
                await _commentsRatingsRepository.AddCommentAsync(comment);
                return Ok(comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("comments/{commentId}", Name = "EditCommentById")]
        public async Task<IActionResult> EditCommentAsync(string commentId, [FromBody] string newContent)
        {
            try
            {
                var success = await _commentsRatingsRepository.EditCommentAsync(commentId, newContent);

                if (success)
                {
                    return Ok("Comment updated successfully");
                }
                else
                {
                    return NotFound("Comment not found");
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("comments/{commentId}", Name = "RemoveCommentById")]
        public async Task<IActionResult> RemoveCommentAsync(string commentId)
        {
            var success = await _commentsRatingsRepository.RemoveCommentAsync(commentId);

            if (success)
            {
                return Ok("Comment removed successfully");
            }
            else
            {
                return NotFound("Comment not found");
            }
        }

        // Ratings Endpoints

        [HttpGet("ratings/{itemId}", Name = "GetRatingsByItemId")]
        public async Task<List<Rating>> GetRatingsByItemId(string itemId)
        {
            var ratings = await _commentsRatingsRepository.GetRatingsByItemId(itemId);
            return ratings;
        }


        [HttpPost("ratings", Name = "AddRating")]
        public async Task<IActionResult> AddRatingAsync([FromBody] Rating rating)
        {
            try
            {
                await _commentsRatingsRepository.AddRatingAsync(rating);
                return Ok(rating);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ratings/{ratingId}", Name = "EditRatingById")]
        public async Task<IActionResult> EditRatingAsync(string ratingId, [FromBody] int newValue)
        {
            try
            {
                var success = await _commentsRatingsRepository.EditRatingAsync(ratingId, newValue);

                if (success)
                {
                    return Ok("Rating updated successfully");
                }
                else
                {
                    return NotFound("Rating not found");
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("ratings/{ratingId}", Name = "RemoveRatingById")]
        public async Task<IActionResult> RemoveRatingAsync(string ratingId)
        {
            var success = await _commentsRatingsRepository.RemoveRatingAsync(ratingId);

            if (success)
            {
                return Ok("Rating removed successfully");
            }
            else
            {
                return NotFound("Rating not found");
            }
        }

    }
}