# Background

Search youtube, rip audio (using yt-dlp) and playback in a music library view.

Search | Play
--- | ---
![Alt text](doc/Screenshot_20230620_201627.png) | ![Alt text](doc/Screenshot_20230620_201731.png)

# Setup
## 1. Prerequisites
### Youtube API key
One of the two search options is through the Youtube API.  
A Google Cloud Project is used te generate the required API key in requests, this is free.  
This is recommended as the second option uses scraping and is prone to breakage if the youtube search results structure were to ever change.

Steps
1. google cloud  
https://console.cloud.google.com/apis  
2. create project
    * (lpane) credentials: create credentials > api key  
    * (lpane) enabled api's & services > + enable api's and services > youtube data api

### yt-dlp
The backend processes downloads through yt-dlp (fork of youtube-dl)  
https://github.com/yt-dlp/yt-dlp

## 2. Configuration
Config files:
* `appsettings.json`  
    * SaveFolder = download location and music library  
    * Shortcut = (optional) name of pwa shortcut in project source, if present will aut ostart  
* `ClientApp/.env`
    * react_app_yt_api_key = (optional) insert Youtube API key here

## 3. Running

```shell
dotnet restore
dotnet build
dotnet run
```

# Deployment

### Local
```shell
# multi platform, runtime required
dotnet publish
# Linux, stand alone
dotnet publish -c linuxrelease --self-contained --runtime linux-x64
# Windows, stand alone
dotnet publish -c windowsrelease --self-contained true --runtime win10-x64
```

### Docker
```yml
#todo
```

# Sources

Based on the .NET 'react' template.

Docker deployment  
https://medium.com/@mustafamagdy1/netcore-react-docker-1d19f051942c  

Audio visualizer  
https://dev.to/ssk14/visualizing-audio-as-a-waveform-in-react-o67   
https://developer.mozilla.org/en-US/docs/Web/API/Web_Audio_API/Visualizations_with_Web_Audio_API