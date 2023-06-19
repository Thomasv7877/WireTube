using System;
using System.Diagnostics;
using System.IO;

namespace dotnet_react_xml_generator;

static class PwaManager {
public static void startPwaShortcut(string shortcut){
    try {
        // open shortcut regardless of extension
        string[] files = Directory.GetFiles(".", shortcut.Split(".")[0] + ".*");
        if(files.Length > 0){
            Process.Start(files[0]);
        }
    } catch(Exception ex){
        Console.WriteLine($"Could not find (pwa) shortcut: {ex.Message}");
    }
}
}