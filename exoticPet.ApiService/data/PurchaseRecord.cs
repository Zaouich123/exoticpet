namespace exoticPet.ApiService.Data;

public class PurchaseRecord
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public Animal Animal { get; set; } = null!;

    public int BuyerId { get; set; }
    public AppUser Buyer { get; set; } = null!;

    public string FullName { get; set; } = "";
    public string Address { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
