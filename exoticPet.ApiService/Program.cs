using Microsoft.EntityFrameworkCore;
using exoticPet.ApiService.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("myapp");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration["Authentication:OIDC:Authority"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "name",
            RoleClaimType = "role",
            ValidAudiences = new[] { "api-test", "account" }
        };
        options.RequireHttpsMetadata = false;
        options.MapInboundClaims = false;
        
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    // Extract roles from realm_access.roles
                    var realmAccessClaim = claimsIdentity.FindFirst("realm_access");
                    if (realmAccessClaim != null)
                    {
                        var realmAccess = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);
                        if (realmAccess.RootElement.TryGetProperty("roles", out var roles))
                        {
                            foreach (var role in roles.EnumerateArray())
                            {
                                claimsIdentity.AddClaim(new Claim("role", role.GetString() ?? ""));
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/environments", async (AppDbContext db) =>
    await db.Environnements.OrderBy(e => e.Name).ToListAsync())
    .RequireAuthorization()
    .WithName("GetEnvironments")
    .WithOpenApi();

app.MapGet("/api/animals", async (AppDbContext db) =>
    await db.Animals
        .Where(a => a.BuyerId == null) // hide purchased animals
        .Include(a => a.Environment)
        .Include(a => a.Buyer)
        .ToListAsync())
    .RequireAuthorization()
    .WithName("GetAnimals")
    .WithOpenApi();

app.MapPost("/api/animals", async (AppDbContext db, Animal animal) =>
{
    var envExists = await db.Environnements.AnyAsync(e => e.Id == animal.EnvironmentId);
    if (!envExists)
    {
        return Results.BadRequest($"Unknown environment id {animal.EnvironmentId}");
    }

    if (animal.BuyerId.HasValue && !await db.Users.AnyAsync(u => u.Id == animal.BuyerId.Value))
    {
        return Results.BadRequest($"Unknown buyer id {animal.BuyerId.Value}");
    }

    db.Animals.Add(animal);
    await db.SaveChangesAsync();
    return Results.Created($"/api/animals/{animal.Id}", animal);
})
.RequireAuthorization()
.Accepts<Animal>("application/json")
.Produces<Animal>(StatusCodes.Status201Created)
.WithName("CreateAnimal")
.WithOpenApi();

app.MapPost("/api/animals/{id:int}/purchase", async (AppDbContext db, int id, HttpContext ctx) =>
{
    // Resolve current user from token name or sub; fallback to user1 if missing
    var userName = ctx.User.Identity?.Name ?? "user1";
    var buyer = await db.Users.FirstOrDefaultAsync(u => u.Username == userName);
    if (buyer == null)
    {
        buyer = new AppUser { Username = userName, Email = $"{userName}@example.com" };
        db.Users.Add(buyer);
        await db.SaveChangesAsync();
    }

    var animal = await db.Animals.FirstOrDefaultAsync(a => a.Id == id);
    if (animal == null) return Results.NotFound();
    if (animal.BuyerId.HasValue) return Results.BadRequest("Animal already purchased");

    animal.BuyerId = buyer.Id;
    await db.SaveChangesAsync();
    return Results.Ok(new { success = true });
})
.RequireAuthorization()
.WithName("PurchaseAnimal")
.WithOpenApi();

app.Run();
