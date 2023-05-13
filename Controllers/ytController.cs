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

    [HttpGet]
    public IEnumerable<string> GetTracks()
    {
        //Console.WriteLine("Called ytApi get");
        //return "Called ytApi get";
        IEnumerable<string> tracks = _ytDlService.getTracks();
        Console.WriteLine(tracks);
        return tracks;
    }
    [HttpPost]
    public IActionResult PostSongUrl([FromBody] string url){
        Console.WriteLine("Downloading: " + url);
        _ytDlService.ripAudio(url);
        return Ok(new { message = "Worked fine" });
    }
}