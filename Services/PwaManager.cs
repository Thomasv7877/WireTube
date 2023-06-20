using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace dotnet_react_xml_generator;

static class PwaManager {
    // open shortcut regardless of extension
    public static void multiPlatOpenShortcut(string shortcut)
    {
        try
        {
            // match shortcut file with appsettings.json shortcut name, ignoring extension
            string[] files = Directory.GetFiles(".", shortcut.Split(".")[0] + ".*");
            if (files.Length > 0)
            {
                string path = files[0];
                // handle Windows
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                // handle Linux
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // opt 1: shortcut open -> NOK, opens text editor
                    //Process.Start("xdg-open", path);
                    // opt 2: extract exec cmd from shortcut file and run from sh -> OK
                    OpenApplication(path);
                    // opt 3: workaround, open webbrowser instead of pwa
                    //Process.Start("xdg-open", "http://localhost:5000");
                }
                else
                {
                    throw new NotSupportedException("Unsupported operating system.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not find (pwa) shortcut: {ex.Message}");
        }
    }

    private static void OpenApplication(string path)
    {
        if (File.Exists(path))
        {
            string execCommand = GetExecCommand(path);
            if (!string.IsNullOrEmpty(execCommand))
            {
                Process.Start("sh", $"-c \"{execCommand}\"");
                return;
            }
        }

        Console.WriteLine("Failed to open the PWA shortcut.");
    }

    private static string GetExecCommand(string path)
    {
        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            if (line.StartsWith("Exec="))
            {
                return line.Substring(5).Trim();
            }
        }

        return null;
    }
}