namespace exoticPet.ApiService.Data;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Species { get; set; } = "";
    public string Specifications { get; set; } = "";
    public decimal Price { get; set; }

    public int EnvironmentId { get; set; }
    public Environnement? Environment { get; set; }

    // Buyer is optional. One user can have many animals, but an animal belongs to at most one user.
    public int? BuyerId { get; set; }
    public AppUser? Buyer { get; set; }
}
