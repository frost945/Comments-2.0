using Comments.Application.Interfaces.Services;
using Comments.Application.Requests;
using Comments.Api.Mappers;
using Comments.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly CommentResponseMapper _responseMapper;
        public CommentController(ICommentService commentService, CommentResponseMapper responseMapper)
        {
            _commentService = commentService;
            _responseMapper = responseMapper;
        }

        // CREATE comment
        [HttpPost]
        public async Task<IActionResult> CreateCommentAsync([FromForm] CommentRequest commentRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createCommentRequest = CommentRequestMapper.ToRequest(commentRequest);

            var commentDto = await _commentService.CreateCommentAsync(createCommentRequest, cancellationToken, createCommentRequest.File);

            var commentResponse = _responseMapper.CreateCommentResponse(commentDto);

            // Return 201 Created with location header pointing to the new comment
            return CreatedAtAction(nameof(GetCommentById), new { id = commentResponse.Id }, commentResponse);
        }

        // GET root (parent) comments
        // api/comments?sortBy=createdAt&ascending=true&skip=0
        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] CommentQuery query, CancellationToken cancellationToken)
        {
            var parentCommentsDto = await _commentService.GetCommentsAsync(query, cancellationToken);

            var parentComments  = parentCommentsDto.Select(_responseMapper.CreateCommentResponse).ToList();

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

            var replies = repliesDto.Select(_responseMapper.CreateCommentResponse).ToList();

            return Ok(replies);
        }

        // GET single comment
        // api/comments/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCommentById(int id, CancellationToken cancellationToken)
        {
            var commentDto = await _commentService.GetCommentById(id, cancellationToken);

            var comment = _responseMapper.CreateCommentResponse(commentDto);

            return Ok(comment);
        }
    }
}
