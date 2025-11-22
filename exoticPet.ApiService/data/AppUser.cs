using System.Text.Json.Serialization;

namespace exoticPet.ApiService.Data;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";

    [JsonIgnore] // Avoid cycles when serializing animals with buyer navigation
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
