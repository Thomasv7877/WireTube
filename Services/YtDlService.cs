using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using NAudio.Wave;

namespace WebApi.Services;

public class YtDlService {

    private readonly AppSettings _appSettings;
    private readonly string? _saveFolder;
    private WaveOutEvent _outputDevice;
    private PlaybackProgressModel _playbackProgressModel;

    public YtDlService(IOptions<AppSettings> appSettingsOptions)
    {
        _appSettings = appSettingsOptions.Value;
        _saveFolder = _appSettings.SaveFolder;
        _outputDevice = new WaveOutEvent();
        _playbackProgressModel = new PlaybackProgressModel();
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
        string[] filenames = Directory.GetFiles(_saveFolder).Select(fullPath => Path.GetFileName(fullPath)).ToArray();

        foreach (string filename in filenames)
        {
            Console.WriteLine(filename);
        }
        return filenames;
    }

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

    
}