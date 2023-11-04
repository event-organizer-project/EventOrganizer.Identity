using IdentityServer4.Models;
using IdentityServer4;
using IdentityModel;

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
                new ApiScope("eventorganizer_api", "Event Organizer API", new[] { JwtClaimTypes.Id }),
                new ApiScope("scheduler_api", "Scheduler API", new[] { JwtClaimTypes.Id })
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("eventorganizer_api")
                {
                    Scopes = { "eventorganizer_api" }
                },
                new ApiResource("scheduler_api")
                {
                    Scopes = { "scheduler_api" }
                }
            };

        public static IEnumerable<Client> GetClients(string[] origin) =>
            new List<Client>
            {
                // React client
                new Client
                {
                    ClientId = "eventorganizer",
                    ClientName = "Event Organizer",
                    ClientUri = origin[0],

                    AllowedGrantTypes = GrantTypes.Implicit,

                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        $"{origin[0]}/signin-oidc",
                    },

                    PostLogoutRedirectUris = { $"{origin[0]}/signout-oidc" },
                    AllowedCorsOrigins = { origin[0] },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "eventorganizer_api"
                    },

                    AllowAccessTokensViaBrowser = true
                },
                // Web Api client
                new Client
                {
                    ClientId = "eventorganizer_api",
                    ClientName = "Event Organizer API",
                    ClientUri = origin[1],

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    RequireClientSecret = false,

                    AllowedCorsOrigins = { origin[1] },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "scheduler_api"
                    },

                    AllowAccessTokensViaBrowser = true
                }
            };
    }
}
