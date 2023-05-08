using Microsoft.AspNetCore.Mvc;
using dotnet_react_xml_generator.Models;

namespace dotnet_react_xml_generator.Controllers;

//[ApiController]
[Route("[controller]")]
public class TriggerController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<TriggerController> _logger;

    public TriggerController(ILogger<TriggerController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<string> Get()
    {
        Console.WriteLine("called get services");
        return Summaries;
    }
    [HttpPost]
    //[Route("/trigger")]
    //vroeger: public async Task<ActionResult<TriggerItem>> PostTriggerItem
    public ActionResult<TriggerItem> PostTriggerItem([FromBody] TriggerItem triggerItem){
        triggerItem.placeOnDisk("/home/thomas/testing/");
        Console.WriteLine(triggerItem.ToString());
        return CreatedAtAction("GetTriggerItem", new { id = triggerItem.service}, triggerItem);
    }
    [HttpPost]
    [Route("/test")]
    public ActionResult<string> PostServiceTest([FromBody] string service){
        //triggerItem.placeOnDisk("/home/thomas/testing/");
        Console.WriteLine("Service: " + service);
        //return Ok();
        return CreatedAtAction("test", new { id = service}, service);
    }
}
