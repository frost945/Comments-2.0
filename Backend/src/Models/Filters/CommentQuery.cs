using Comments.Models.Enums;

namespace Comments.Models.Filters
{
    public class CommentQuery
    {
        public int Skip { get; set; } = 0;
        public readonly int PageSize = 25;
        public CommentSortField SortBy { get; set; } = CommentSortField.createdAt;
        public bool Ascending { get; set; } = true;

        // keyset pagination
        public int? CursorId { get; set; }
        public DateTime? CursorCreatedAt { get; set; }
        public int? Sign { get; set; } = 1; // 1 for next page, -1 for prev page
    }
}
