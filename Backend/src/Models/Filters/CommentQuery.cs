using Comments.Models.Enums;

namespace Comments.Models.Filters
{
    public class CommentQuery
    {
        public int Skip { get; set; } = 0;
        public int PageSize { get; set; } = 25;
        public CommentSortField SortBy { get; set; } = CommentSortField.createdAt;
        public bool Ascending { get; set; } = true;
    }
}
