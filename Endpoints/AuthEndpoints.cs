using ReelTrack.Models;
using ReelTrack.Services;

namespace ReelTrack.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", async (RegisterRequest request, AuthService auth) =>
        {
            var response = await auth.Register(request);
            if (response == null)
                return Results.BadRequest("Email already exists");
            return Results.Ok(response);
        })
        .WithName("Register")
        .WithOpenApi();

        app.MapPost("/auth/login", async (LoginRequest request, AuthService auth) =>
        {
            var response = await auth.Login(request);
            if (response == null)
                return Results.BadRequest("Invalid credentials");
            return Results.Ok(response);
        })
        .WithName("Login")
        .WithOpenApi();

        app.MapPost("/auth/dev-token", async (AuthService auth) =>
        {
            if (app.Environment.IsDevelopment())
            {
                var devUser = new User
                {
                    Id = 1,
                    Email = "dev@local",
                    IsAdmin = true,
                    FamilyId = 1
                };
                var token = auth.GenerateDevToken(devUser);
                return Results.Ok(new { Token = token });
            }
            return Results.NotFound();
        })
        .WithName("DevToken")
        .WithOpenApi();
    }
}