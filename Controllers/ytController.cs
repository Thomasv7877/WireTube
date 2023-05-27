using Microsoft.AspNetCore.Mvc;
using WebApi.Services;
using Newtonsoft.Json;

using System.Collections.Concurrent;

namespace dotnet_react_xml_generator.Controllers;

//[ApiController]
[Route("[controller]")]
public class YtApiController : ControllerBase
{
    private readonly ILogger<TriggerController> _logger;
    private readonly YtDlService _ytDlService;
    private static readonly ConcurrentDictionary<string, SSEClient> Clients = new ConcurrentDictionary<string, SSEClient>();
    private static readonly Timer Timer = new Timer(SendUpdate, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

    public YtApiController(ILogger<TriggerController> logger, YtDlService ytDlService)
    {
        _logger = logger;
        _ytDlService = ytDlService;
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
    [HttpPost]
    public IActionResult PostSongUrl([FromBody] string url){
        Console.WriteLine("Downloading: " + url);
        //_ytDlService.ripAudio(url);
        _ytDlService.ripAudioWProgress(url);
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
    [Route("dlprogresssocket")]
    public IActionResult SSE()
    {
        var response = HttpContext.Response;
        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Connection", "keep-alive");

        var client = new SSEClient(response);
        Clients.TryAdd(client.Id, client);

        response.OnCompleted(() =>
        {
            Clients.TryRemove(client.Id, out _);
            return Task.CompletedTask;
        });

        return new EmptyResult();
    }

    private static void SendUpdate(object state)
    {
        var update = new { Message = "Progress update", Progress = DateTime.Now.Millisecond };
        //string message = "event: eventName\ndata: This is the message data\n";
        var serializedUpdate = JsonConvert.SerializeObject(update);

        foreach (var client in Clients.Values)
        {
            client.Send(serializedUpdate);
        }
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

        public void Send(string data)
        {
            //_response.WriteAsync($"event: update,\n");
            _response.WriteAsync($"event: update\ndata: testData\n\n");
            //_response.WriteAsync($"id: {_eventId++}\n\n");
            _response.Body.Flush();
        }
    }
    
}