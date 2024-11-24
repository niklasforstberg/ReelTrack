namespace ReelTrack.Models;

public class MovieWatch
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int WatchListId { get; set; }
    public WatchList WatchList { get; set; } = null!;
    public int ChoosenById { get; set; }
    public User ChoosenBy { get; set; } = null!;
    public DateTime WatchedDate { get; set; }
}