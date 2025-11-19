namespace exoticPet.WebApp.Clients;

public class AnimalClient : IAnimalClient
{
    private readonly HttpClient _httpClient;

    public AnimalClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Animal>> GetAnimalsAsync()
    {
        try
        {
            Console.WriteLine($"[v0] Fetching animals from {_httpClient.BaseAddress}/api/animals");
            return await _httpClient.GetFromJsonAsync<List<Animal>>("/api/animals") ?? new List<Animal>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[v0] Error fetching animals: {ex.Message}");
            throw;
        }
    }

    public async Task<Animal> CreateAnimalAsync(Animal animal)
    {
        try
        {
            Console.WriteLine($"[v0] Attempting to POST animal to {_httpClient.BaseAddress}/api/animals");
            Console.WriteLine($"[v0] Animal data: Name={animal.Name}, Species={animal.Species}");
            
            var response = await _httpClient.PostAsJsonAsync("/api/animals", animal);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[v0] POST failed with status {response.StatusCode}: {content}");
                throw new HttpRequestException($"Failed to create animal: {response.StatusCode}", null, response.StatusCode);
            }
            
            Console.WriteLine($"[v0] Animal created successfully");
            return await response.Content.ReadFromJsonAsync<Animal>() ?? throw new Exception("Failed to deserialize animal response");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[v0] HTTP Error: {ex.StatusCode} - {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[v0] Error creating animal: {ex.Message}");
            throw;
        }
    }
}
