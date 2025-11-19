using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace exoticPet.WebApp.Clients;

public class TokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new Exception("HttpContext not available");
        }

        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            Console.WriteLine($"[v0] TokenHandler: Added Bearer token to request");
        }
        else
        {
            Console.WriteLine("[v0] TokenHandler: No access token found in context");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
