using Comments.Application.Interfaces;
using Comments.Contracts;
using Comments.Models.Enums;
using Comments.Models.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // CREATE comment
        [HttpPost]
        public async Task<IActionResult> CreateCommentAsync([FromForm] CommentRequest commentRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentService.CreateCommentAsync(commentRequest, commentRequest.File);

            // Return 201 Created with location header pointing to the new comment
            return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
        }

        // GET root (parent) comments
        // api/comments?sortBy=createdAt&ascending=true&skip=0
        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] CommentQuery query)
        {
            var parentComments = await _commentService.GetCommentsAsync(query);

            return Ok(parentComments);
        }

        // GET replies (children) of comment
        // api/comments/{id}/replies?skip=0
        [HttpGet("{id:int}/replies")]
        public async Task<IActionResult> GetReplies(int id, [FromQuery] int skip = 0)
        {
            var query = new CommentQuery { Skip = skip };

            var replies = await _commentService.GetCommentsAsync(query, parentId: id);

            return Ok(replies);
        }

        // GET single comment
        // api/comments/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var comment = await _commentService.GetCommentById(id);

            return Ok(comment);
        }
    }
}
