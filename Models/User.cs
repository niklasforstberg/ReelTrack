using System.ComponentModel.DataAnnotations;

namespace ReelTrack.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? FamilyId { get; set; }
    public Family? Family { get; set; }
    public bool IsAdmin { get; set; }
    public List<WatchList> WatchLists { get; set; } = new();
}