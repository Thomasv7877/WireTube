using System;
using System.Diagnostics;
using System.IO;

namespace dotnet_react_xml_generator;

static class PwaManager {

    public static void startPwaShortcut(){
        Process process = new Process();
        process.StartInfo.FileName = "pwa_shortcut";
        process.Start();
        //process.WaitForExit();
    }
}