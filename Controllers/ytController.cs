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
    private static SSEClient? _activeClient;
    //private static readonly Timer Timer = new Timer(SendUpdate, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    private readonly Queue<string> _eventQueue = new Queue<string>();
    private readonly IHostApplicationLifetime _applicationLifetime;

    public YtApiController(ILogger<TriggerController> logger, YtDlService ytDlService, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _ytDlService = ytDlService;
        //_ytDlServiceWProgress = ytDlServiceWProgress;
        _applicationLifetime = applicationLifetime;
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
    public async Task<IActionResult> PostSongUrl([FromBody] JsonElement dlItem){
        string url = dlItem.GetProperty("url").GetString();
        string title = dlItem.GetProperty("title").GetString();
        Console.WriteLine("Downloading vid: " + url + "\nwith title:"  + title);
        //_ytDlService.ripAudio(url);
        var downloader = new YtDlServiceWProgress(title);
        downloader.DownloadProgressChanged += (progress) =>
        {
            string pgstr = $"Progress: {progress}% voor vid {downloader._vidTitle}";
            Console.WriteLine(pgstr);
            _eventQueue.Enqueue(pgstr);
            SendUpdate(downloader._vidTitle, progress);
        };
        await _ytDlService.ripAudioWProgress(url, downloader);
        //Clients.Clear();
        //Console.WriteLine("Cleared clients");
        return Ok(new { message = "Worked fine" });
    }

    [HttpPost("search")]
    public async Task<IActionResult> searchytnoapi([FromBody] string url){
        Console.WriteLine("Handled url: " + url);
        //string? res =  await YtSearchService.searchYt(url);
        IEnumerable<dynamic> response = await YtSearchService.searchYtAlt(url);
        //await YtSearchService.searchYtTest(url);
        //Console.WriteLine("Handled function: " + res);
        //return Ok(new {url = url});
        return Ok(response);
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

        _activeClient = new SSEClient(response);
        //Clients.TryAdd(_activeClient.Id, _activeClient);
        Console.WriteLine("Attempting client connection in socket.. with client present? " + Clients.Count);

        HandleShutdown();

        while(_activeClient != null){
        //Console.WriteLine("Running progress loop, actieve dl's: " + _ytDlService.ActiveDownloads());
        if (_eventQueue.TryDequeue(out var eventData)){
            /*_ytDlService.progressList.ForEach( activedl => {
            SendUpdate((int) activedl._progress, activedl._vidTitle);
            });*/
            Console.WriteLine("Dequeue worked!");
            //SendUpdate(eventData);
        }
            await Task.Delay(500);
        }
              
        response.OnCompleted(() =>
        {
            //Clients.TryRemove(_activeClient.Id, out _);
            //_activeClient = null;
            Console.WriteLine("Disposed of client");
            return Task.CompletedTask;
        });
        //Timer.Dispose();
        return new EmptyResult();
    }

    private static void SendUpdate(string title, int progress)
    {
        //Console.WriteLine("Ran Timer callback");
        var update = new { Title = title, Progress = progress};
        //string message = "event: eventName\ndata: This is the message data\n";
        var serializedUpdate = JsonConvert.SerializeObject(update);

        /*foreach (var client in Clients.Values)
        {
            Console.WriteLine("Client present");
            client.Send(serializedUpdate);
        }*/
        _activeClient.Send(serializedUpdate);
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
        public async void CloseClient(){
                await _response.Body.FlushAsync();
                await _response.CompleteAsync();
                _activeClient = null;
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
        Console.WriteLine("Stop logic called");
        // Unregister the event handler
        Console.CancelKeyPress -= Console_CancelKeyPress;
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
    
