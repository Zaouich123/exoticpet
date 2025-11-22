namespace exoticPet.WebApp.Clients;

public interface IEnvironmentClient
{
    Task<List<Environnement>> GetEnvironmentsAsync();
}

public class EnvironmentClient : IEnvironmentClient
{
    private readonly HttpClient _httpClient;

    public EnvironmentClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Environnement>> GetEnvironmentsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Environnement>>("/api/environments")
               ?? new List<Environnement>();
    }
}
