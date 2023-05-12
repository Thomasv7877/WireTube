using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace WebApi.Services;

public class YtDlService {

    private readonly AppSettings _appSettings;

    public YtDlService(IOptions<AppSettings> appSettingsOptions)
    {
        _appSettings = appSettingsOptions.Value;
    }
    public void ripAudio (string vidUrl){
        //string map = "/mnt/crucial/music";
        string? map = _appSettings.SaveFolder;
        string command = $"youtube-dl -o '{map}/%(title)s-%(artist)s.%(ext)s' -x --embed-thumbnail --add-metadata {vidUrl}";

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "youtube-dl",
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
}