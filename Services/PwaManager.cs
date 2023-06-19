using System;
using System.Diagnostics;
using System.IO;

namespace dotnet_react_xml_generator;

static class PwaManager {
public static void startPwaShortcut(string shortcut){
    try {
        // open shortcur regardless of extension
        string[] files = Directory.GetFiles(".", shortcut.Split(".")[0]);
        if(files.Length > 0){
            Console.WriteLine("File gevonden: " + files[0]);
            Process.Start(shortcut);
        }
    } catch(Exception ex){
        Console.WriteLine($"Could not find (pwa) shortcut: {ex.Message}");
    }
}
}