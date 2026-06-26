using System.ComponentModel;

namespace Comments.Contracts.Enums
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
