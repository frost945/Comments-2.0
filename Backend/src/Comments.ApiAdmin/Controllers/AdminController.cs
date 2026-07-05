using Comments.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public AdminController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            bool isDelete = await _commentService.DeleteByIdAsync(id, ct);
            if (!isDelete)
                return NotFound();

            return NoContent();
        }
    }
}