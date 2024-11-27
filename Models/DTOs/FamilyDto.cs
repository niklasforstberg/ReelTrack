
namespace ReelTrack.Models.DTOs;
public class FamilyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string InviteCode { get; set; }
    public List<UserDto> Members { get; set; } = new();
}