using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
//using NAudio.Wave;
//using NAudio.opus;
//using NAudio.Wave.SampleProviders;
//using NetCoreAudio;
// using System.Timers;
// using System;
// using System.Media;
//using System.Timers;
//using TagLib;
using TagLib;
using WebApi.Services;

namespace WebApi.Services;

public class YtDlService {

    private readonly AppSettings _appSettings;
    private readonly string? _saveFolder;
    //private WaveOutEvent _outputDevice;
    private PlaybackProgressModel _playbackProgressModel;
    //private AudioFileReader _audioFile;
    //private Player _player;
    //private System.Timers.Timer _timer;

    public YtDlService(IOptions<AppSettings> appSettingsOptions)
    {
        _appSettings = appSettingsOptions.Value;
        _saveFolder = _appSettings.SaveFolder;
        //_outputDevice = new WaveOutEvent();
        _playbackProgressModel = new PlaybackProgressModel();
        //_timer = new System.Timers.Timer(1000);

    }
    public void ripAudio (string vidUrl){
        //string map = "/mnt/crucial/music";
        
        //string saveUrl = $"";
        string command = $"-o \"{_saveFolder}/%(title)s - %(artist)s.%(ext)s\" -x --add-metadata {vidUrl}";
        Console.WriteLine(command);

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments = command,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        Console.WriteLine(output);
    }
    public void ripAudioWProgress (string vidUrl){
        string command = $"--newline --no-warnings --no-call-home -o \"{_saveFolder}/%(title)s - %(artist)s.%(ext)s\" -x -r 100k --add-metadata {vidUrl}";
        Console.WriteLine(command);
        var downloader = new YtDlServiceWProgress();
        downloader.DownloadProgressChanged += progress =>
        {
            Console.WriteLine($"Progress: {progress}%");
        };
        downloader.DownloadVideo(command);
    }
    public IEnumerable<dynamic> getTracks(){
        var extensions = new string[]{".opus", ".mp3", ".ogg", ".wav"};

        var songs = Directory.GetFiles(_saveFolder)
            .Where(f => extensions.Any(ext => Path.GetExtension(f) == ext))
            .Select( filePath => TagLib.File.Create(filePath))
            .Select(tagLib => new {
                FileName = Path.GetFileName(tagLib.Name),
                Title = tagLib.Tag.Title,
                Album = tagLib.Tag.Album,
                Artist = tagLib.Tag.FirstArtist,
                Duration = tagLib.Properties.Duration.ToString(@"mm\:ss"),
                SampleRate = tagLib.Properties.AudioSampleRate,
                Codec = tagLib.Properties.Codecs.FirstOrDefault().Description.Split(" ").First(),
                FileSize = Math.Round((double)(new FileInfo(Path.Combine(_saveFolder, tagLib.Name)).Length) / (1024 * 1024), 2)
            }
            ).ToArray();

        return songs;
    }
    public IEnumerable<string> getTrackNames(){
        var extensions = new string[]{".opus", ".mp3", ".ogg", ".wav"};
        string[] filenames = Directory.GetFiles(_saveFolder).Where(f => extensions.Any(ext => Path.GetExtension(f) == ext)).Select(fullPath => Path.GetFileName(fullPath)).ToArray();

        /*foreach (string filename in filenames)
        {
            Console.WriteLine(filename);
        }*/
        Console.WriteLine($"found {filenames.Length} audio files");

        return filenames;
    }

    public FileStream? GetFileStream(string fileName){
        string filePath = Path.Combine(_saveFolder, fileName);
        // Check if the file exists
        if (!System.IO.File.Exists(filePath)){
            return null;
        }

        // Open the file stream
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return fileStream;
    }
    public string GetContentType(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);

            // Map the file extension to the corresponding content type
            switch (fileExtension.ToLower())
            {
                case ".mp3":
                    return "audio/mpeg";
                case ".ogg":
                    return "audio/ogg";
                case ".wav":
                    return "audio/wav";
                case ".opus":
                    return "audio/opus";
                // Add more cases as needed for different audio file formats
                default:
                    return null; // Unsupported file extension
            }
        }

/*
    public void PlaySong(string fileName){

        _playbackProgressModel.SongId = fileName;
        _playbackProgressModel.IsPlaying = true;
        string filePath = Path.Combine(_saveFolder, fileName);

        using (var audioFile = new AudioFileReader(filePath))
        {
            _outputDevice.Init(audioFile);
            _outputDevice.Play();
            
            // You can add additional logic here if needed
            // For example, you can wait until the song finishes playing before continuing
            while (_outputDevice.PlaybackState == PlaybackState.Playing)
            {
                System.Threading.Thread.Sleep(100);
                _playbackProgressModel.CurrentPosition = audioFile.CurrentTime;
            }
        }
}
    public void StopSong(){
        _playbackProgressModel.IsPlaying = false;
        _outputDevice.Stop();
    }
    public TimeSpan GetSongProgress(){
        return (TimeSpan) _playbackProgressModel.CurrentPosition;
    }
    */
    }
    // Niet nodig, dynmamisch obj gebruikt in getTracks
    public class songInfo{
        string FileName {get; set;}
        string Title {get; set;}
        string Album {get; set;}
        string Artist {get; set;}
        TimeSpan Duration {get; set;}
        string SampleRate {get; set;}
        string Codec {get; set;}
        int FileSize {get; set;}

    }