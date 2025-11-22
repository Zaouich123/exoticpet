namespace exoticPet.WebApp.Clients;

public class ProfileResponse
{
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public List<PurchaseInfo> Purchases { get; set; } = new();
}

public class PurchaseInfo
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public PurchaseAnimalInfo Animal { get; set; } = new();
}

public class PurchaseAnimalInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Environment { get; set; }
}
