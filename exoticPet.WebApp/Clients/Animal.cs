namespace exoticPet.WebApp.Clients;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Specifications { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int EnvironmentId { get; set; }
    public Environnement? Environment { get; set; }
}

public class Environnement
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
