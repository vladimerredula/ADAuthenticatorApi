using ADAuthenticatorApi.Models;
using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using System.Net;

namespace ADAuthenticatorApi.Services
{
    public class LdapAuthService
    {
        private readonly LdapSettings _ldap;

        public LdapAuthService(IOptions<LdapSettings> options)
        {
            _ldap = options.Value;
        }

        public (bool Success, IDictionary<string, string> Attributes) AuthenticateAndGetAttributes(string username, string password)
        {
            try
            {
                var identifier = new LdapDirectoryIdentifier(_ldap.Host, _ldap.Port);
                var credential = new NetworkCredential($"{username}@{_ldap.Domain}", password);

                using var connection = new LdapConnection(identifier)
                {
                    AuthType = AuthType.Basic
                };

                connection.SessionOptions.ProtocolVersion = 3;
                connection.SessionOptions.SecureSocketLayer = false;

                // This will throw if credentials are invalid  
                connection.Bind(credential);

                var searchRequest = new SearchRequest(
                    $"OU={_ldap.Domain},{_ldap.BaseDn}",
                    $"(sAMAccountName={username})",
                    SearchScope.Subtree,
                    ["employeeID", "mail", "displayName"] // Attributes to retrieve  
                );

                var response = (SearchResponse)connection.SendRequest(searchRequest);

                if (response.Entries.Count == 1)
                {
                    var entry = response.Entries[0];
                    var attributes = new Dictionary<string, string>();

                    string? GetAttr(string attrName) =>
                        entry.Attributes[attrName]?.Count > 0 ? entry.Attributes[attrName][0].ToString() : null;

                    attributes["employeeID"] = GetAttr("employeeID") ?? "";
                    attributes["email"] = GetAttr("mail") ?? "";
                    attributes["displayName"] = GetAttr("displayName") ?? "";

                    return (true, attributes);
                }

                return (false, new Dictionary<string, string>());
            }
            catch (LdapException ex)
            {
                Console.WriteLine($"LdapException: {ex.Message}");
                return (false, new Dictionary<string, string>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General exception: {ex.Message}");
                return (false, new Dictionary<string, string>());
            }
        }
    }
}
