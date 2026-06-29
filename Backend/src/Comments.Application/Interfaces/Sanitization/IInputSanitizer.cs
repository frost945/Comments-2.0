namespace Comments.Application.Interfaces.Sanitization
{
    public interface IInputSanitizer
    {
        string SanitizeComment(string input);
        string SanitizeUsername(string input);
        string SanitizeEmail(string input);
    }
}
