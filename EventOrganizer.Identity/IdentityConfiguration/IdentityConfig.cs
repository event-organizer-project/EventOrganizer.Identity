using IdentityServer4.Models;
using IdentityServer4;

namespace EventOrganizer.Identity.IdentityConfiguration
{
    public static class IdentityConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("eventorganizerapi", "Event Organizer API")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("eventorganizerapi")
                {
                    Scopes = { "eventorganizerapi" }
                }
            };

        public static IEnumerable<Client> GetClients(string origin) =>
            new List<Client>
            {
                // React client
                new Client
                {
                    ClientId = "eventorganizer",
                    ClientName = "Event Organizer",
                    ClientUri = origin,

                    AllowedGrantTypes = GrantTypes.Implicit,

                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        $"{origin}/signin-oidc",
                    },

                    PostLogoutRedirectUris = { $"{origin}/signout-oidc" },
                    AllowedCorsOrigins = { origin },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "eventorganizerapi"
                    },

                    AllowAccessTokensViaBrowser = true
                }
            };
    }
}
