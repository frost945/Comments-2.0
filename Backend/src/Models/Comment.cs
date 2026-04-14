using System.Net.Mail;

namespace Comments.Models
{
    public class Comment
    {
        public int Id { get; private set; }
        public int? ParentId { get; private set; }
        public List<Comment> Children { get; private set; } = new();
        public Comment? Parent { get; set; } // Navigation property for parent comment
        public string UserName { get; private set; }=string.Empty;
        public string Email { get; private set; }=string.Empty;
        public string Text { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public Guid? ImageId { get; private set; }
        public Guid? TextFileId { get; private set; }
        public string? OriginalTextFileName { get; private set; }

        private Comment() { }

        public Comment(int? parentId, string userName, string email, string text, Guid? imageId, Guid? textFileId, string? originalTextFileName)
        {
            ParentId = parentId;
            SetUserName(userName);
            SetEmail(email);
            SetText(text);
            ImageId = imageId;
            TextFileId = textFileId;
            OriginalTextFileName = originalTextFileName;
        }

        private void SetUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length < 3 || userName.Length > 20)
                throw new ArgumentException("Invalid username");

            UserName = userName;
        }

        private void SetEmail(string email)
        {
            ValidateEmail(email);

            Email = email;
        }

        private void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 2000)
                throw new ArgumentException("Invalid text");

            Text = text;
        }

        public static void ValidateEmail(string email)
        {
            try
            { _ = new MailAddress(email);}

            catch
            {throw new ArgumentException("Invalid email");}
        }

    }
}
