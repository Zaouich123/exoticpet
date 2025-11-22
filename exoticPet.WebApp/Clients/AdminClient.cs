namespace exoticPet.WebApp.Clients;

public interface IAdminClient
{
    Task<List<AdminPurchase>> GetPurchasesAsync();
    Task<bool> CancelPurchaseAsync(int purchaseId);
}

public class AdminClient : IAdminClient
{
    private readonly HttpClient _httpClient;

    public AdminClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AdminPurchase>> GetPurchasesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<AdminPurchase>>("/api/admin/purchases")
               ?? new List<AdminPurchase>();
    }

    public async Task<bool> CancelPurchaseAsync(int purchaseId)
    {
        var response = await _httpClient.PostAsync($"/api/admin/purchases/{purchaseId}/cancel", null);
        return response.IsSuccessStatusCode;
    }
}

public class AdminPurchase
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public PurchaseAnimalInfo Animal { get; set; } = new();
    public AdminBuyer Buyer { get; set; } = new();
}

public class AdminBuyer
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
}
