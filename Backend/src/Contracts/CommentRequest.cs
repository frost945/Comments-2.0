using System.ComponentModel.DataAnnotations;

namespace Comments.Contracts
{
    public class CommentRequest
    {
        public int? ParentId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Text { get; set; } = null!;

        public IFormFile? File { get; set; }
    }
}
