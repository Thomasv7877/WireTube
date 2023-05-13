using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace WebApi.Services;

public class YtDlService {

    private readonly AppSettings _appSettings;
    private readonly string? _saveFolder;

    public YtDlService(IOptions<AppSettings> appSettingsOptions)
    {
        _appSettings = appSettingsOptions.Value;
        _saveFolder = _appSettings.SaveFolder;
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
        string[] filenames = Directory.GetFiles(_saveFolder);

        foreach (string filename in filenames)
        {
            Console.WriteLine(filename);
        }
        return filenames;
    }
}