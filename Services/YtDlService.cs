using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using TagLib;
using WebApi.Services;

namespace WebApi.Services;

public class YtDlService {

    private readonly AppSettings _appSettings;
    private readonly string? _saveFolder;
    public List<YtDlServiceWProgress> progressList;

    public YtDlService(IOptions<AppSettings> appSettingsOptions)
    {
        _appSettings = appSettingsOptions.Value;
        _saveFolder = _appSettings.SaveFolder;
        progressList = new List<YtDlServiceWProgress>();

    }
    // build yt-dlp command, actual dl via YtDlServiceWProgress instance
    public async Task ripAudioWProgress (string vidUrl, YtDlServiceWProgress downloader){
        string command = $"--newline --no-warnings --no-call-home -o \"{_saveFolder}/%(title)s - %(artist)s.%(ext)s\" -x -f bestaudio -r 350k --add-metadata {vidUrl}";
        Console.WriteLine(command);
        progressList.Add(downloader);
        // change between cmd's DownloadVideo or DownloadVideoDummy (testing)
        await downloader.DownloadVideoDummy(command);
    }
    // fetch audio info: var songs = filter by ext > get metadata via TabLib > create object to return to frontend
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
    // get only the filenames
    public IEnumerable<string> getTrackNames(){
        var extensions = new string[]{".opus", ".mp3", ".ogg", ".wav"};
        string[] filenames = Directory.GetFiles(_saveFolder)
            .Where(f => extensions.Any(ext => Path.GetExtension(f) == ext))
            .Select(fullPath => Path.GetFileName(fullPath))
            .ToArray();

        Console.WriteLine($"found {filenames.Length} audio files");

        return filenames;
    }
    // stream song to frontend
    public FileStream? GetFileStream(string fileName){
        string filePath = Path.Combine(_saveFolder, fileName);
        if (!System.IO.File.Exists(filePath)){
            return null;
        }

        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return fileStream;
    }
    // determine content type in stream to frontend, needed in audio endpoint FileStreamResult return obj
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
                default:
                    return null; // Unsupported file extension
            }
        }
    public int ActiveDownloads(){
        return progressList.Count;
    }
}