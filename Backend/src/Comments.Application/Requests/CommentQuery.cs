using Comments.Application.Requests.Enums;

namespace Comments.Application.Requests
{
    public class CommentQuery
    {
        public int Skip { get; set; } = 0;
        public readonly int PageSize = 25;
        public CommentSortField SortBy { get; set; } = CommentSortField.createdAt;
        public bool Ascending { get; set; } = true;

        // keyset pagination - only sort by createdAt field
        public int? CursorId { get; set; }
        public DateTime? CursorCreatedAt { get; set; }
        public bool? Direction { get; set; } = true; // true for next page, false for prev page

       // public int PageNumber => (Skip / PageSize) + 1; // used to determine the number of cached pages
    }
}
