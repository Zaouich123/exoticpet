namespace exoticPet.WebApp.Clients;

public interface IAnimalClient
{
    Task<List<Animal>> GetAnimalsAsync();
    Task<Animal> CreateAnimalAsync(Animal animal);
}
