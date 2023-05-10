using Microsoft.AspNetCore.Mvc;

namespace dotnet_react_xml_generator.Controllers;

//[ApiController]
[Route("[controller]")]
public class YtApiController : ControllerBase
{
    private readonly ILogger<TriggerController> _logger;

    public YtApiController(ILogger<TriggerController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string Get()
    {
        Console.WriteLine("Called ytApi get");
        return "Called ytApi get";
    }
    [HttpPost]
    public ActionResult<string> PostTriggerItem([FromBody] string testVar){
        Console.WriteLine(testVar);
        return Ok(new { message = "Worked fine" });
    }
}