using System;
using System.Diagnostics;
using System.IO;

public class YtDlServiceWProgress
{
    public event Action<int> DownloadProgressChanged;

    public void DownloadVideo(string ytDlpArgs)
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

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            // Parse the output to extract the progress value
            string output = e.Data;
            //Console.WriteLine($"Raw progress output: {output} %");
            if (output.Contains("download") && output.Contains("%"))
            {
                int startIndex = output.IndexOf("download") + 10;
                int endIndex = output.IndexOf("% of ~") - 2;
                if (startIndex >= 0 && endIndex >= 0)
                {
                    string progressString = output.Substring(startIndex, endIndex - startIndex).Trim();
                    //Console.WriteLine($"Trimmed output: {progressString} %");
                    if (int.TryParse(progressString, out int progress))
                    {
                        // Raise the DownloadProgressChanged event
                        //Console.WriteLine($"Progress: {progress} %");
                        DownloadProgressChanged?.Invoke(progress);
                    }
                }
            }
        }
    }
}
