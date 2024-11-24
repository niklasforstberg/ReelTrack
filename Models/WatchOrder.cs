namespace ReelTrack.Models;

public class WatchOrder
{
    public int Id { get; set; }
    public int WatchListId { get; set; }
    public WatchList WatchList { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int Order { get; set; }
}