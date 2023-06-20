using Microsoft.AspNetCore.Mvc;
using WebApi.Services;
using Newtonsoft.Json;

using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace dotnet_react_xml_generator.Controllers;

//[ApiController]
[Route("[controller]")]
public class YtApiController : ControllerBase
{
    private readonly ILogger<YtApiController> _logger;
    private readonly YtDlService _ytDlService;
    private static readonly ConcurrentDictionary<string, SSEClient> Clients = new ConcurrentDictionary<string, SSEClient>();
    private static SSEClient? _activeClient;
    private readonly Queue<string> _eventQueue = new Queue<string>();
    private readonly IHostApplicationLifetime _applicationLifetime;

    public YtApiController(ILogger<YtApiController> logger, YtDlService ytDlService, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _ytDlService = ytDlService;
        _applicationLifetime = applicationLifetime;
    }
    // return info for audio files in library dir
    [HttpGet("tracks")]
    public IEnumerable<dynamic> GetTracks()
    {
        return _ytDlService.getTracks();
    }
    // return song names
    [HttpGet("tracknames")]
    public IEnumerable<string> GetTrackNames()
    {
        return _ytDlService.getTrackNames();
    }
    // start download operation (yt-dlp), progress made available through sse
    [HttpPost("dl")]
    public async Task<IActionResult> PostSongUrl([FromBody] JsonElement dlItem){
        string? url = dlItem.GetProperty("url").GetString();
        string? title = dlItem.GetProperty("title").GetString();
        Console.WriteLine("Downloading vid: " + url + "\nwith title:"  + title);
        var downloader = new YtDlServiceWProgress(title);
        downloader.DownloadProgressChanged += (progress) =>
        {
            string pgstr = $"Progress: {progress}% voor vid {downloader._vidTitle}";
            Console.WriteLine(pgstr);
            _eventQueue.Enqueue(pgstr);
            SendUpdate(downloader._vidTitle, progress);
        };
        await _ytDlService.ripAudioWProgress(url, downloader);
        return Ok(new { message = "Download complete" });
    }
    // search yt through scraping
    [HttpPost("search")]
    public async Task<IActionResult> searchytnoapi([FromBody] string url){
        Console.WriteLine("Handled url: " + url);
        IEnumerable<dynamic> response = await YtSearchService.searchYtAlt(url);
        return Ok(response);
    }
    // stream audio files to frontend
    [HttpGet]
    [Route("audio")]
    public IActionResult StreamSong(string fileName)
    {
        Console.WriteLine("streaming song: " + fileName);
        FileStream? fileStream = _ytDlService.GetFileStream(fileName);
        if (fileStream == null){
                return NotFound();
            }

        string contentType = _ytDlService.GetContentType(fileName);
        if (contentType == null){
            return BadRequest();
        }
        
        return new FileStreamResult(fileStream, contentType);
    }
    // create new SSE connection, updates are sent through callback of previous endpoint though, so plenty of unused code, kept as reference
    [HttpGet]
    [Route("dlprogress")]
    public async Task<IActionResult> SSE()
    {
        var response = HttpContext.Response;
        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Connection", "keep-alive");
        //response.Headers.Add("Access-Control-Allow-Origin", "*");

        _activeClient = new SSEClient(response);
        //Clients.TryAdd(_activeClient.Id, _activeClient); // when multiple clients, unused
        Console.WriteLine("Attempting client connection in socket.. with client present? " + Clients.Count);

        HandleShutdown();

        while(_activeClient != null){
            // sinding updates through below loop doesn't work, multithreading issue
            if (_eventQueue.TryDequeue(out var eventData)){
                /*_ytDlService.progressList.ForEach( activedl => {
                SendUpdate((int) activedl._progress, activedl._vidTitle);
                });*/
                Console.WriteLine("Dequeue worked!");
                //SendUpdate(eventData);
            }
            await Task.Delay(500);
        }
              
        // HandleShutdown() will handle removal of client through ctrl+c signal, below is unused
        response.OnCompleted(() =>
        {
            //Clients.TryRemove(_activeClient.Id, out _);
            //_activeClient = null;
            //Console.WriteLine("Disposed of client");
            return Task.CompletedTask;
        });
        return new EmptyResult();
    }

    // end of the endpoints, below are helper functions //

    // send back an update to the frontend with song title and process precentage
    private static void SendUpdate(string title, int progress)
    {
        var update = new { Title = title, Progress = progress};
        var serializedUpdate = JsonConvert.SerializeObject(update);
        _activeClient.Send(serializedUpdate);
    }
    // manage SSE client behavior
    private class SSEClient
    {
        private readonly HttpResponse _response;
        private int _eventId;

        public string Id { get; }

        public SSEClient(HttpResponse response)
        {
            _response = response;
            Id = Guid.NewGuid().ToString();
        }
        // sends the actual http response, called by SendUpdate
        public async void Send(string data)
        {
            await _response.WriteAsync($"event: update\n");
            await _response.WriteAsync($"id: {_eventId++}\n");
            await _response.WriteAsync($"data: {data}\n\n");
            //await _response.WriteAsync("data: test\n\n");
            await _response.Body.FlushAsync();
        }
        // dispose of client, if not done explicitly shutdown hangs a few seconds
        public async void CloseClient(){
                await _response.Body.FlushAsync();
                await _response.CompleteAsync();
                _activeClient = null;
        }
    }

    // workaround, handle ctrl+c handler for disposing of open sse connection, else slowness on shutdown
    public IActionResult HandleShutdown()
    {
        Console.CancelKeyPress += Console_CancelKeyPress;
        
        Console.WriteLine("Ctrl+C signal handling registered.");
        return Ok("Ctrl+C signal handling registered.");
    }
    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        if(_activeClient != null)
        {
        _activeClient.CloseClient();
        }
        _applicationLifetime.StopApplication();
        //Console.WriteLine("Stop logic called");
        // Unregister the event handler
        Console.CancelKeyPress -= Console_CancelKeyPress;
    }

    // unused, reference example for basic sse endpoint
    /*[HttpGet]
    [Route("ssetest")]
    public async Task SSETest(){

            var response = HttpContext.Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            // Simulate progress updates (replace with your own logic)
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000); // Simulate delay between updates

                var message = $"Progress: {i * 10}%\n\n";

                await response.WriteAsync("data: test\n\n");
                await response.Body.FlushAsync();
            }
    }*/

    }
    
