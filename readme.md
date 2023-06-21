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
This is recommended as the second option uses scraping and is prone to breaking if the youtube search results structure were to ever change.

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

Optional: auto start pwa 

# Deployment

### 1. Local
```shell
# multi platform, runtime required
dotnet publish
# Linux, stand alone
dotnet publish -c linuxrelease --self-contained true --runtime linux-x64
# Windows, stand alone
dotnet publish -c windowsrelease --self-contained true --runtime win10-x64
```

**Optional**: make a PWA and configure for auto start

1. Create the PWA  
browse to localhost:5000, then..
    * Chrome: 'three dots' > more tools > create shortcut > name 'pwa_shortcut' and check 'Open as window'
    * Edge: 'three dots' > Apps > install this site as an app > name 'pwa_shortcut'
    * Firefox: not supported

2. Place the created shortcut in the project root  
Example:
![Alt text](doc/Screenshot_20230621_145455.png)

### 2. Docker

Example Dockerfile and docker-compose.yml included.

* [Dockerfile](Dockerfile)

* [docker-compose.yml](docker-compose.yml)

# Functionality:

List
* React frontend <-> .NET web api backend (communication to and from)
* Passing of download progress to front (sse endpoint | alt would be: websockets, SignalR)
* Execute yt-dlp download (start Process and redirect output)
* Audio visualizer (Audio Web API)
* Search Youtube using the Youtube API
* Search youtube without Youtube API = scraping (HtmlAgilityPack lib)
* Get audio file info from the backend (TagLibSharp lib)
* Multi platform auto start of PWA shortcuts

# Sources

Based on the .NET 'react' template.

Docker deployment  
https://medium.com/@mustafamagdy1/netcore-react-docker-1d19f051942c  

Audio visualizer  
https://dev.to/ssk14/visualizing-audio-as-a-waveform-in-react-o67   
https://developer.mozilla.org/en-US/docs/Web/API/Web_Audio_API/Visualizations_with_Web_Audio_API

Youtube API (video search)  
https://developers.google.com/youtube/v3/docs/search/list?apix_params=%7B%22part%22%3A%5B%22classical%20music%22%5D%7D

Html Agility Pack (traversing results)  
https://html-agility-pack.net/traversing

TagLibSharp  
https://github.com/mono/taglib-sharp