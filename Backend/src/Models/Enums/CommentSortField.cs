using System.ComponentModel;

namespace Comments.Models.Enums
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
