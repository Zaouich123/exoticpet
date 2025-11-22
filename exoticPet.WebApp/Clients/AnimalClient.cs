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
            Console.WriteLine($"[v0] Animal data: Name={animal.Name}, Species={animal.Species}, EnvId={animal.EnvironmentId}, Price={animal.Price}");
            
            var response = await _httpClient.PostAsJsonAsync("/api/animals", animal);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"[v0] Animal created successfully");
            return await response.Content.ReadFromJsonAsync<Animal>()
                   ?? throw new InvalidOperationException("Invalid response from server");
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

    public async Task<bool> PurchaseAnimalAsync(int animalId, PurchaseRequest request)
    {
        try
        {
            Console.WriteLine($"[v0] Attempting to purchase animal {animalId}");
            var response = await _httpClient.PostAsJsonAsync($"/api/animals/{animalId}/purchase", request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[v0] Purchase HTTP Error: {ex.StatusCode} - {ex.Message}");
            return false;
        }
    }
}
