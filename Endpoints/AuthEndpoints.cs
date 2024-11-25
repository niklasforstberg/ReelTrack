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
    }
}