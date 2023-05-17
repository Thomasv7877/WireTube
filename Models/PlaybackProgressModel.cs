public class PlaybackProgressModel
{
    public string? SongId { get; set; }
    public TimeSpan? CurrentPosition { get; set; }
    public bool IsPlaying { get; set; }

    public PlaybackProgressModel(){
    }

    public PlaybackProgressModel(string SongId){
        this.SongId = SongId;
        CurrentPosition = new TimeSpan();
        IsPlaying = true;
    }
}