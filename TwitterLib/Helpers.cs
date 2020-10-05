using System.Text.RegularExpressions;

namespace TwitterLib
{
    public static class Helpers
    {
        public static string UrlRegEx { get; private set; } = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";

        public static string HashtagRegEx { get; private set; } = @"#\w+";

        public static MatchCollection GetMatches(string data, string regEx)
        {

            var regex = new Regex(regEx);
            var matches = regex.Matches(data);

            return matches;
        }
    }
}
