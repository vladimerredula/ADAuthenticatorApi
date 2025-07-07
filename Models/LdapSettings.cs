namespace ADAuthenticatorApi.Models
{
    public class LdapSettings
    {
        public string Host { get; set; } = "";
        public string Domain { get; set; } = "";
        public int Port { get; set; } = 389;
        public string BaseDn { get; set; } = "";
    }
}
