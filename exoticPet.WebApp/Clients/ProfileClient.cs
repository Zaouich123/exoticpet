namespace exoticPet.WebApp.Clients;

public interface IProfileClient
{
    Task<ProfileResponse?> GetProfileAsync();
}

public class ProfileClient : IProfileClient
{
    private readonly HttpClient _httpClient;

    public ProfileClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProfileResponse?> GetProfileAsync()
    {
        return await _httpClient.GetFromJsonAsync<ProfileResponse>("/api/profile");
    }
}
