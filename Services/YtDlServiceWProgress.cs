using System;
using System.Diagnostics;
using System.IO;

public class YtDlServiceWProgress
{
    public event Action<int> DownloadProgressChanged;
    public int? _progress;
    public string? _vidTitle;

    public YtDlServiceWProgress(string vidTitle){
        _vidTitle = vidTitle;
        _progress = 0;
    }
    // start a process to run the yt-dlp command, output (progress part) is captured and made available through DownloadProgressChanged callback
    public async Task DownloadVideo(string ytDlpArgs)
    {
        // Set up the process start info
        var startInfo = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments = ytDlpArgs,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Create and start the process
        var process = new Process();
        process.StartInfo = startInfo;
        process.OutputDataReceived += Process_OutputDataReceived;
        process.Start();
        process.BeginOutputReadLine();

        process.WaitForExit();
    }
    // for testing frontend dl progress view, every half second tick up 5%
    public async Task DownloadVideoDummy(string ytDlpArgs){
        for(var i = 0; i <= 100; i += 5){
            _progress = i;
            //Console.WriteLine($"Progress (manual): {_progress}% voor vid {_vidTitle}");
            DownloadProgressChanged?.Invoke((int) _progress);
            await Task.Delay(1000);
        }
    }
    // handler on process output, each line download percentage is filtered out and made available with DownloadProgressChanged
    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            // Parse the output to extract the progress value
            string output = e.Data;
            if (output.Contains("download") && output.Contains("%"))
            {
                int startIndex = output.IndexOf("download") + 10;
                int endIndex = output.IndexOf("% of ~") - 2;
                if (startIndex >= 0 && endIndex >= 0)
                {
                    string progressString = output.Substring(startIndex, endIndex - startIndex).Trim();
                    if (int.TryParse(progressString, out int progress))
                    {
                        // Raise the DownloadProgressChanged event
                        DownloadProgressChanged?.Invoke(progress);
                    }
                }
            }
        }
    }
}
