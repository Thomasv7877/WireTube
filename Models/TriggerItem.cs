namespace dotnet_react_xml_generator.Models;

using System.Xml.Linq;
using System;

public class TriggerItem
{
    public DateTime startDate {get; set;}
    public DateTime endDate {get; set;}
    public string? service {get; set;}

    public void placeOnDisk (string saveDir){
        Console.WriteLine( $"Called placeOnDisk to dir: {saveDir}");
        XDocument doc = new XDocument(
            new XElement("Trigger",
                new XElement("Service", service),
                new XElement("StartTime", startDate),
                new XElement("EndTime", endDate)
            )
        );
        Guid guid = Guid.NewGuid();
        doc.Save($"{saveDir}/{service}-{guid}.xml");
    }
    public override string ToString()
    {
        return $"Service: {service}\nStart: {startDate}\nEnd: {endDate}";
    }

}