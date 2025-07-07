namespace ADAuthenticatorApi.Models
{
    public class JwtSettings
    {
        public string Key { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int ExpireMinutes { get; set; } = 60; // Default to 60 minutes
    }
}
