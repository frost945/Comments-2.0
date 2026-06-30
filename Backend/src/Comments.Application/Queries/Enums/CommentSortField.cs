using System.ComponentModel;

namespace Comments.Application.Queries.Enums
{
    public enum CommentSortField
    {
        [Description("Created At")]
        createdAt,

        [Description("User Name")]
        userName,

        [Description("Email")]
        email
    }
}
