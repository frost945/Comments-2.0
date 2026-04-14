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

        [HttpPost]
        public async Task<IActionResult> AddCommentAsync([FromForm] CommentRequest commentRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentService.CreateCommentAsync(commentRequest, commentRequest.File);
            return Ok(comment);
        }

        [HttpGet("parent")]
        public async Task<IActionResult> GetAllParentComments(CommentSortField sortBy = CommentSortField.createdAt, bool ascending = true, int skip=0)
        {
            var query = new CommentQuery
            {
                Skip = skip,
                SortBy = sortBy,
                Ascending = ascending
            };

            var parentComments = await _commentService.GetCommentsAsync(query);

            return Ok(parentComments);
        }

        [HttpGet("children")]
        public async Task<IActionResult> GetChildrenCommentsAsync(int parentId, int skip = 0)
        {
            var query = new CommentQuery { Skip = skip };

            var childrenComments = await _commentService.GetCommentsAsync(query, parentId);

            return Ok(childrenComments);
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var comment = await _commentService.GetCommentById(id);

            return Ok(comment);
        }
    }
}
