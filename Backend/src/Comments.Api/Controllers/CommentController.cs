using Comments.Api.Mappers;
using Comments.Application.Interfaces.Services;
using Comments.Contracts;
using Comments.Contracts.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly CommentResponseMapper _mapper;
        public CommentController(ICommentService commentService, CommentResponseMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        // CREATE comment
        [HttpPost]
        public async Task<IActionResult> CreateCommentAsync([FromForm] CommentRequest commentRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var commentDto = await _commentService.CreateCommentAsync(commentRequest, cancellationToken, commentRequest.File);
            var comment = _mapper.CreateCommentResponse(commentDto);

            // Return 201 Created with location header pointing to the new comment
            return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
        }

        // GET root (parent) comments
        // api/comments?sortBy=createdAt&ascending=true&skip=0
        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] CommentQuery query, CancellationToken cancellationToken)
        {
            var parentCommentsDto = await _commentService.GetCommentsAsync(query, cancellationToken);

            var parentComments  = parentCommentsDto.Select(_mapper.CreateCommentResponse).ToList();

            return Ok(parentComments);
        }

        // GET replies (children) of comment
        // api/comments/{id}/replies?skip=0
        [HttpGet("{id:int}/replies")]
        public async Task<IActionResult> GetReplies(int id, string? cursorCreatedAt, int? cursorId, CancellationToken cancellationToken)
        {
            var query = new CommentQuery
            {
                CursorCreatedAt = string.IsNullOrEmpty(cursorCreatedAt) ? null : DateTime.Parse(cursorCreatedAt),
                CursorId =  cursorId
            };

            var repliesDto = await _commentService.GetCommentsAsync(query, cancellationToken, parentId: id);

            var replies = repliesDto.Select(_mapper.CreateCommentResponse).ToList();

            return Ok(replies);
        }

        // GET single comment
        // api/comments/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCommentById(int id, CancellationToken cancellationToken)
        {
            var commentDto = await _commentService.GetCommentById(id, cancellationToken);

            var comment = _mapper.CreateCommentResponse(commentDto);

            return Ok(comment);
        }
    }
}
