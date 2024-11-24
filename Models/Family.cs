namespace ReelTrack.Models;

public class Family
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public List<User> Members { get; set; } = new();
    public List<WatchList> WatchLists { get; set; } = new();
}