using System.ComponentModel;

namespace Comments.Application.Requests.Enums
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
