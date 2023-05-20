using Microsoft.AspNetCore.Mvc;
using WebApi.Services;
using Newtonsoft.Json;

namespace dotnet_react_xml_generator.Controllers;

//[ApiController]
[Route("[controller]")]
public class YtApiController : ControllerBase
{
    private readonly ILogger<TriggerController> _logger;
    private readonly YtDlService _ytDlService;

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
        _ytDlService.ripAudio(url);
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
    
}