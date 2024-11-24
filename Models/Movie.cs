namespace ReelTrack.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImdbId { get; set; } = string.Empty;
    public List<MovieWatch> Watches { get; set; } = new();
}