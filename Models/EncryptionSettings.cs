namespace ADAuthenticatorApi.Models
{
    public class EncryptionSettings
    {
        public string SecretKey { get; set; } = "";
        public string InitVector { get; set; } = "";
    }
}
