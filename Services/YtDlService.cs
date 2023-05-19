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
    public IEnumerable<string> getTracks(){
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
        //System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
        //Console.WriteLine($"File info:\n{fileInfo.CreationTime}");
        TagLib.File file = TagLib.File.Create(filePath);
        TagLib.Properties properties = file.Properties;

        Console.WriteLine("File Name: " + System.IO.Path.GetFileName(filePath));
        Console.WriteLine("Duration: " + properties.Duration);
        Console.WriteLine("Bitrate: " + properties.AudioBitrate);
        Console.WriteLine("Sample Rate: " + properties.AudioSampleRate);
        Console.WriteLine("Channels: " + properties.AudioChannels);
        Console.WriteLine("Codec Name: " + properties.Codecs.FirstOrDefault()?.Description);


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