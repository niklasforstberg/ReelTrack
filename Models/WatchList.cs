namespace ReelTrack.Models;

public class WatchList
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FamilyId { get; set; }
    public Family Family { get; set; } = null!;
    public List<User> Members { get; set; } = new();
    public List<WatchOrder> WatchOrder { get; set; } = new();
    public List<MovieWatch> MovieWatches { get; set; } = new();
}