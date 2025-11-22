using System.Text.Json.Serialization;

namespace exoticPet.ApiService.Data;

public class Environnement
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    [JsonIgnore] // Prevent cycles when serializing Animals -> Environment -> Animals...
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
