namespace Comments.Application.Interfaces.Services
{
    public interface IInputSanitizationService
    {
        string SanitizeComment(string input);
        string SanitizeUsername(string input);
        string SanitizeEmail(string input);
    }
}
