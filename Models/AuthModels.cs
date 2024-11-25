namespace ReelTrack.Models;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string Name);
public record AuthResponse(string Token, int UserId, string Email);