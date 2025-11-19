using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using exoticPet.WebApp.Clients;
using exoticPet.WebApp.Components;
using exoticPet.WebApp.Extensions;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceDefaults for Aspire integration
builder.AddServiceDefaults();

// Add Razor Components
builder.Services.AddRazorComponents();

// Add HttpContextAccessor for token handling
builder.Services.AddHttpContextAccessor();

// Configure Authentication with Cookies + OIDC
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "exoticpet.auth";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration["Authentication:OIDC:Authority"];
    options.ClientId = builder.Configuration["Authentication:OIDC:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:OIDC:ClientSecret"];
    options.RequireHttpsMetadata = false;
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";
    
    options.UseTokenLifetime = false;
    options.MapInboundClaims = false;
    
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("api");
    options.Scope.Add("offline_access");
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role",
        ValidateIssuer = true,
        ValidateAudience = false,
    };
    
    options.ClaimActions.MapUniqueJsonKey("name", "name");
    options.ClaimActions.MapUniqueJsonKey("email", "email");
    options.ClaimActions.MapUniqueJsonKey("sub", "sub");
    
    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            Console.WriteLine($"[v0] Redirecting to: {context.ProtocolMessage.IssuerAddress}");
            Console.WriteLine($"[v0] Redirect URI: {context.ProtocolMessage.RedirectUri}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[v0] Auth Error: {context.Exception.Message}");
            Console.WriteLine($"[v0] Stack: {context.Exception.StackTrace}");
            context.Response.Redirect($"/error?message={Uri.EscapeDataString(context.Exception.Message)}");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("[v0] Token validated successfully");
            
            if (context.Principal?.Identity is System.Security.Claims.ClaimsIdentity identity)
            {
                var realmAccessClaim = identity.FindFirst("realm_access");
                if (realmAccessClaim != null)
                {
                    try
                    {
                        var realmAccess = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);
                        if (realmAccess.RootElement.TryGetProperty("roles", out var rolesElement))
                        {
                            foreach (var role in rolesElement.EnumerateArray())
                            {
                                identity.AddClaim(new System.Security.Claims.Claim("role", role.GetString() ?? ""));
                                Console.WriteLine($"[v0] Added role: {role.GetString()}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[v0] Error parsing realm_access: {ex.Message}");
                    }
                }
            }
            
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            Console.WriteLine($"[v0] Remote failure: {context.Failure?.Message}");
            context.Response.Redirect($"/error?message={Uri.EscapeDataString(context.Failure?.Message ?? "Unknown error")}");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

// Configure cookie refresh for token renewal
builder.Services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddTransient<exoticPet.WebApp.Clients.TokenHandler>();

builder.Services.AddHttpClient<IAnimalClient, AnimalClient>(client =>
{
    // This ensures the POST request goes to the correct API endpoint without Blazor interception
    client.BaseAddress = new Uri("http://localhost:5251");
})
.AddHttpMessageHandler<exoticPet.WebApp.Clients.TokenHandler>();

var app = builder.Build();

// Map default Aspire endpoints
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>();

app.MapGet("/authentication/login", (HttpContext context, string? returnUrl) => 
{
    var properties = new AuthenticationProperties
    {
        RedirectUri = returnUrl ?? "/"
    };
    return Results.Challenge(properties, [OpenIdConnectDefaults.AuthenticationScheme]);
}).AllowAnonymous();

app.MapGet("/authentication/logout", async (HttpContext context) =>
{
    var properties = new AuthenticationProperties
    {
        RedirectUri = "/"
    };
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, properties);
    return Results.Redirect("/");
}).RequireAuthorization();

app.MapGet("/error", (HttpContext context) =>
{
    var message = context.Request.Query["message"].ToString();
    var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Authentication Error</title>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; max-width: 800px; margin: 0 auto; }}
        h1 {{ color: #d32f2f; }}
        .error-box {{ background: #ffebee; padding: 20px; border-radius: 4px; margin: 20px 0; }}
        .btn {{ display: inline-block; padding: 10px 20px; background: #1976d2; color: white; text-decoration: none; border-radius: 4px; }}
    </style>
</head>
<body>
    <h1>Authentication Error</h1>
    <div class='error-box'>
        <p><strong>There was a problem signing in.</strong></p>
        {(string.IsNullOrEmpty(message) ? "" : $"<p>Error details: {System.Web.HttpUtility.HtmlEncode(message)}</p>")}
    </div>
    <h3>Troubleshooting:</h3>
    <ul>
        <li>Check that Keycloak is running on <code>http://localhost:8090</code></li>
        <li>Verify the client 'blazor-client' exists in realm 'exoticpet'</li>
        <li>Ensure Valid Redirect URIs include: <code>http://localhost:5000/*</code></li>
        <li>Check the browser console and server logs for more details</li>
    </ul>
    <a href='/' class='btn'>Go Home</a>
</body>
</html>";
    return Results.Content(html, "text/html");
}).AllowAnonymous();

app.Run();
