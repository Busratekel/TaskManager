namespace Helpers
{
    public static class EmailHelper
    {
        public static string NormalizeEmails(string? emails)
        {
            if (string.IsNullOrEmpty(emails)) return string.Empty;

            return string.Join(";", 
                emails.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim().ToLower())
                    .Where(IsValidEmail)
                    .Distinct()
                    .OrderBy(e => e));
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string[] GetEmailList(string? emails) => 
            !string.IsNullOrEmpty(emails) 
                ? emails.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .ToArray() 
                : Array.Empty<string>();
    }
} 