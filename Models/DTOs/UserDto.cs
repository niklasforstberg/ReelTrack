namespace ReelTrack.Models.DTOs;

public record UserDto(
    int Id,
    string Email,
    string Name,
    bool IsAdmin,
    int? FamilyId
);