using ReelTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ReelTrack.Helpers;
using ReelTrack.Models.DTOs;

namespace ReelTrack.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app, IConfiguration config)
    {
        app.MapPost("/auth/register", async (RegisterRequest request, ReelTrackDbContext context) =>
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
                return Results.BadRequest("Email already exists");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Name = request.Name
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = GenerateToken(user, config);
            return Results.Ok(new AuthResponse(token, user.Id, user.Email));
        })
        .WithName("Register")
        .WithOpenApi();


        app.MapPost("/auth/login", async (LoginRequest request, ReelTrackDbContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Results.BadRequest("Invalid credentials");

            var token = GenerateToken(user, config);
            return Results.Ok(new AuthResponse(token, user.Id, user.Email));
        })
        .WithName("Login")
        .WithOpenApi();

        app.MapPost("/auth/dev-token", (IConfiguration config) =>
        {
            if (app.Environment.IsDevelopment())
            {
                var devUser = new User
                {
                    Id = 1,
                    Email = "dev@example.com",
                    IsAdmin = true,
                    FamilyId = 1
                };
                var token = GenerateToken(devUser, config, TimeSpan.FromDays(365));
                return Results.Ok(new { Token = token });
            }
            return Results.NotFound();
        })
        .WithName("DevToken")
        .WithOpenApi();

        app.MapGet("/auth/me", async (HttpContext context, ReelTrackDbContext dbContext) =>
        {
            try
            {
                Console.WriteLine("Claims:");
                foreach (var claim in context.User.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

                var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    Console.WriteLine("Unauthorized: Email claim not found.");
                    return Results.Unauthorized();
                }

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    Console.WriteLine($"Not Found: No user with email {email}.");
                    return Results.NotFound($"No user with email {email}.");
                }

                return Results.Ok(new UserDto(
                    user.Id,
                    user.Email,
                    user.Name,
                    user.IsAdmin,
                    user.FamilyId
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Results.Problem("Internal server error.");
            }
        })
        .WithName("GetCurrentUser")
        .WithOpenApi()
        .RequireAuthorization();
    }

    private static string HashPassword(string password)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)) + ":" +
               Convert.ToBase64String(SHA256.HashData(
                   System.Text.Encoding.UTF8.GetBytes(password)));
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var parts = hash.Split(':');
        var computedHash = Convert.ToBase64String(SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
        return parts[1] == computedHash;
    }

    private static string GenerateToken(User user, IConfiguration config, TimeSpan? expiry = null)
    {
        var claims = new List<Claim>
        {
            new("email", user.Email),
            new("id", user.Id.ToString()),
            new("isAdmin", user.IsAdmin.ToString()),
            new("familyId", user.FamilyId?.ToString() ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine(jwt);
        return jwt;
    }
}