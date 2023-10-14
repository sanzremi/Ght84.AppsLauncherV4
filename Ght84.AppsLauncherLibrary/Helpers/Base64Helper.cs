namespace Ght84.AppsLauncherLibrary.Helpers
{
    public static class Base64Helper
    {
        public static string Base64Encode(string text)
        {
            if (!string.IsNullOrEmpty(text)) { 
                var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
                return System.Convert.ToBase64String(textBytes);
            }
            else
            {
                return string.Empty;
            }

        }
        public static string Base64Decode(string base64)
        {
            if (!string.IsNullOrEmpty(base64))
            {
                var base64Bytes = System.Convert.FromBase64String(base64);
                return System.Text.Encoding.UTF8.GetString(base64Bytes);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}