namespace ReelTrack.Models;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string Name);
public record AuthResponse(string Token, int UserId, string Email);

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}