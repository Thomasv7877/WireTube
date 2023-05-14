public class PlaybackProgressModel
{
    public string? SongId { get; set; }
    public TimeSpan? CurrentPosition { get; set; }
    public bool IsPlaying { get; set; }
}