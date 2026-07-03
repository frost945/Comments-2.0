using Microsoft.AspNetCore.Http;

namespace Comments.Application.Requests
{
    public class CreateCommentRequest
    {
        public int? ParentId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Text { get; set; } = null!;
        public IFormFile? File { get; set; }
    }
}
