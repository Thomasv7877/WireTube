using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

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
    public string Get()
    {
        Console.WriteLine("Called ytApi get");
        return "Called ytApi get";
    }
    [HttpPost]
    public ActionResult<string> PostTriggerItem([FromBody] string testVar){
        Console.WriteLine("Downloading: " + testVar);
        _ytDlService.ripAudio(testVar);
        return Ok(new { message = "Worked fine" });
    }
}