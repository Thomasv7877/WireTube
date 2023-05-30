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
    private readonly ILogger<TriggerController> _logger;
    private readonly YtDlService _ytDlService;
    //private YtDlServiceWProgress _ytDlServiceWProgress;
    private static readonly ConcurrentDictionary<string, SSEClient> Clients = new ConcurrentDictionary<string, SSEClient>();
    //private static readonly Timer Timer = new Timer(SendUpdate, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

    public YtApiController(ILogger<TriggerController> logger, YtDlService ytDlService)
    {
        _logger = logger;
        _ytDlService = ytDlService;
        //_ytDlServiceWProgress = ytDlServiceWProgress;
    }

    [HttpGet("tracks")]
    public IEnumerable<dynamic> GetTracks()
    {
        //Console.WriteLine("Called ytApi get");
        //return "Called ytApi get";
        //IEnumerable<string> dummySongs = new []{"song1", "song2"};
        var tracks = _ytDlService.getTracks();
        //Console.WriteLine(tracks);
        return tracks;
    }
    [HttpGet("tracknames")]
    public IEnumerable<string> GetTrackNames()
    {
        //Console.WriteLine("Called ytApi get");
        //return "Called ytApi get";
        //IEnumerable<string> dummySongs = new []{"song1", "song2"};
        IEnumerable<string> tracks = _ytDlService.getTrackNames();
        //Console.WriteLine(tracks);
        return tracks;
    }
    [HttpPost("dl")]
    public IActionResult PostSongUrl([FromBody] JsonElement dlItem){
        string url = dlItem.GetProperty("url").GetString();
        string title = dlItem.GetProperty("title").GetString();
        Console.WriteLine("Downloading vid: " + url + "\nwith title:"  + title);
        //_ytDlService.ripAudio(url);
        _ytDlService.ripAudioWProgress(url, title);
        return Ok(new { message = "Worked fine" });
    }
    [HttpGet] // [HttpGet("play")]
    [Route("play")]
    public IActionResult PlayMusic(string fileName)
    {
        Console.WriteLine("trying song playback for: " + fileName);
        //_ytDlService.PlaySong(fileName);
        
        return Ok();
    }
    [HttpGet] // [HttpGet("{fileName}")]
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
    [HttpGet]
    [Route("dlprogress")]
    public async Task<IActionResult> SSE()
    {
        
        var response = HttpContext.Response;
        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Connection", "keep-alive");
        //response.Headers.Add("Access-Control-Allow-Origin", "*");

        var client = new SSEClient(response);
        Clients.TryAdd(client.Id, client);
        Console.WriteLine("Attempting client connection in socket.. with client present? " + Clients.Count);

        while(_ytDlService.ActiveDownloads() > 0){
        //Console.WriteLine("Running progress loop, actieve dl's: " + _ytDlService.ActiveDownloads());
        if(_ytDlService.ActiveDownloads() > 0){
            _ytDlService.progressList.ForEach( activedl => {
            SendUpdate((int) activedl._progress, activedl._vidTitle);
            });
        }
            await Task.Delay(1000);
        }
              
        response.OnCompleted(() =>
        {
            Clients.TryRemove(client.Id, out _);
            Console.WriteLine("Disposed of client");
            return Task.CompletedTask;
        });
        //Timer.Dispose();
        return new EmptyResult();
    }

    private static void SendUpdate(int progress, string title)
    {
        Console.WriteLine("Ran Timer callback");
        var update = new { Message = "Progress update", Progress = progress , Title = title};
        //string message = "event: eventName\ndata: This is the message data\n";
        var serializedUpdate = JsonConvert.SerializeObject(update);

        foreach (var client in Clients.Values)
        {
            Console.WriteLine("Client present");
            client.Send(serializedUpdate);
        }
    }

    private static void SendUpdateTest(object state){
        Console.WriteLine("Test timer callback");
    }

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

        public async void Send(string data)
        {
            Console.WriteLine("Sent some data..");
            await _response.WriteAsync($"event: update\n");
            await _response.WriteAsync($"id: {_eventId++}\n");
            await _response.WriteAsync($"data: {data}\n\n");
            //await _response.WriteAsync("data: test\n\n");
            await _response.Body.FlushAsync();
        }
    }

     [HttpGet]
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
    }
    /* // event handler voor progress, wait op cancellation token ipv loop wait -> werkt niet
    [HttpGet]
    [Route("dlprogressalt")]
    public async Task GetProgress()
    {
        var context = HttpContext;

        // Set the response headers for SSE
        context.Response.Headers.Add("Content-Type", "text/event-stream");
        context.Response.Headers.Add("Cache-Control", "no-cache");
        context.Response.Headers.Add("Connection", "keep-alive");
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

        // Create a CancellationToken to stop the SSE connection if needed
        var cancellationToken = context.RequestAborted;

        // Subscribe to the ProgressChanged event
        Action<int> progressHandler = async (progress) =>
        {
            var eventData = $"data: {progress}\n\n";
            Console.WriteLine("Writing progress: " + progress);
            // Write the SSE data to the response
            await context.Response.WriteAsync(eventData);

            // Flush the response to ensure immediate transmission
            await context.Response.Body.FlushAsync();
        };

        _ytDlServiceWProgress.DownloadProgressChanged += progressHandler;

        try
        {
            // Keep the SSE connection open until it is canceled
            cancellationToken.WaitHandle.WaitOne();
            while(!cancellationToken.IsCancellationRequested){
                await Task.Delay(1000);
            }
        }
        finally
        {
            // Unsubscribe from the ProgressChanged event
            _ytDlServiceWProgress.DownloadProgressChanged -= progressHandler;
        }
    }*/

    }
    
