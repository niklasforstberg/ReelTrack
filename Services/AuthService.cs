using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ReelTrack.Helpers;
using ReelTrack.Models;

namespace ReelTrack.Services;

public class AuthService
{
    private readonly ReelTrackDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(ReelTrackDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return null;

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Email);
    }

    public async Task<AuthResponse?> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        var user = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Name = request.Name
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Email);
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
}