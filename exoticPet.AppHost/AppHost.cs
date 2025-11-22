var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "16")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("myapp");

var keycloakDb = postgres.AddDatabase("keycloak");

var keycloak = builder.AddContainer("keycloak-service", "quay.io/keycloak/keycloak", "latest")
    .WithHttpEndpoint(port: 8090, targetPort: 8080, name: "http")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    .WithEnvironment("KC_DB", "postgres")
    .WithEnvironment("KC_DB_URL", "jdbc:postgresql://postgres:5432/keycloak")
    .WithEnvironment("KC_DB_USERNAME", "postgres")
    .WithEnvironment("KC_DB_PASSWORD", "zAwE6DBy~)SJZS-770)yzF")
    .WithArgs("start-dev")
    .WithLifetime(ContainerLifetime.Persistent);

var apiService = builder.AddProject<Projects.exoticPet_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database)
    .WaitFor(keycloak);

builder.AddProject<Projects.exoticPet_WebApp>("webapp")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
