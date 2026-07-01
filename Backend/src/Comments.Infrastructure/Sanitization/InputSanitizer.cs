using Comments.Application.Interfaces.Sanitization;
using Ganss.Xss;
using System.Text.RegularExpressions;
using System.Web;

namespace Comments.Infrastructure.Sanitization
{
    public class InputSanitizer : IInputSanitizer
    {
        private readonly HtmlSanitizer _commentSanitizer = CreateSanitizer();

        private static HtmlSanitizer CreateSanitizer()
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("a");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("code");
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("href");
            sanitizer.AllowedAttributes.Add("title");
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");
            return sanitizer;
        }

        public string SanitizeComment(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = HttpUtility.HtmlDecode(input);

            string sanitized = _commentSanitizer.Sanitize(input);

            sanitized = sanitized.Trim();

            return sanitized;
        }

        public string SanitizeUsername(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = HttpUtility.HtmlDecode(input);

            // Remove all Html tags
            input = Regex.Replace(input, "<.*?>", "");

            // Disable Unicode controls (invisible, null, etc.)
            input = RemoveControlCharacters(input);

            // We only allow letters, numbers, and ., _, -
            input = Regex.Replace(input, @"[^a-zA-Z0-9а-яА-ЯёЁ_\-.]", "");

            return input;
        }

        public string SanitizeEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = HttpUtility.HtmlDecode(input);

            // Remove all Html tags
            input = Regex.Replace(input, "<.*?>", "");

            // Disable Unicode controls (invisible, null, etc.)
            input = RemoveControlCharacters(input);

            input = input.Trim();
            input = input.ToLowerInvariant();

            return input;
        }

        private static string RemoveControlCharacters(string input)
        {
            return new string(input.Where(c => !char.IsControl(c)).ToArray());
        }
    }
}