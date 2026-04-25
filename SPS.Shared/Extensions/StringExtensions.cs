namespace SPS.Shared.Extensions
{
 
    public static class StringExtensions
    {
        #region String Operation
        public static string AppendToURL(this string baseURL, params string[] segments) =>
            string.Join("/", new[] { baseURL.TrimEnd('/') }.Concat(segments.Select(s => s.Trim('/'))));
        #endregion

        #region String Validation
        public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
        public static bool IsNumeric(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return long.TryParse(str, out _);
        }
        #endregion
    }
}
