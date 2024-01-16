using EventOrganizer.Identity.IdentityConfiguration;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration.GetValue<bool>("UseCustomSslCertificates"))
{
    var certs = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase)
    {
        ["localhost"] = new X509Certificate2("/app/certificates/localhost.pfx", "password"),
        ["host.docker.internal"] = new X509Certificate2("/app/certificates/host.docker.internal.pfx", "password")
    };

    using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser))
    {
        store.Open(OpenFlags.ReadWrite);

        store.Add(certs["localhost"]);
        store.Add(certs["host.docker.internal"]);
    }

    builder.WebHost.ConfigureKestrel(options =>
        options.ConfigureHttpsDefaults(opt =>
        {
            opt.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            opt.ServerCertificateSelector = (connectionContext, name) =>
                name == "host.docker.internal" ? certs["host.docker.internal"] : certs["localhost"]; ;
        }));
}

var origins = new[]
{
    builder.Configuration.GetValue<string>("AllowedOrigins:WebClient"),
    builder.Configuration.GetValue<string>("AllowedOrigins:WebApi"),
    builder.Configuration.GetValue<string>("AllowedOrigins:SchedulerClient")
};

builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
    .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
    .AddInMemoryClients(IdentityConfig.GetClients(origins))
    .AddInMemoryApiResources(IdentityConfig.ApiResources)
    .AddTestUsers(TestUsers.Users)
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All ^ HttpLoggingFields.RequestHeaders;
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// TO DO: Remove this section after configuring ForwardedHeaders 
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next(context);
});

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();

app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.UseHttpLogging();

app.UseCors();

app.Run();
