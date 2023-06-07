using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.Json;
using System.IO;

namespace WebApi.Services;

public static class YtSearchService {

    /*public async static Task<string> searchYtRaw (string url){
        //HttpClient ipv curl
        HttpClient httpClient = new HttpClient();

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return "OK";
    }*/

    public async static Task<string> searchYt (string url){
        string className = "style-scope ytd-video-renderer";

        List<HtmlNode> divs = await FetchDivsWithClassName(url, className);

        if(divs.Count > 0){
            /*foreach (HtmlNode div in divs)
            {
                // Do something with each div
                Console.WriteLine(div.InnerHtml);
            }*/
            //Console.WriteLine(divs.Last().InnerHtml);
            var filePath = "/home/thomas/Documenten/YtSearchService.json";

            //string name = parsedObject["name"];
            // largest script file has sezarch results
            var pseudoJson = divs.Last().InnerHtml;
            // '{}' + substring of script content - var name and ';' at the end = parseable json object
            int varLength = 20;
            //dynamic? jsonObject = JsonSerializer.Deserialize<dynamic>(pseudoJson.Substring(varLength, pseudoJson.Length - (varLength + 1)));
            //string somePart = jsonObject["responseContext"]["serviceTrackingParams"];
            JsonDocument document = JsonDocument.Parse(pseudoJson.Substring(varLength, pseudoJson.Length - (varLength + 1)));
            JsonElement root = document.RootElement;
            string somePart = root.GetProperty("contents").GetProperty("twoColumnSearchResultsRenderer").GetProperty("primaryContents").GetProperty("sectionListRenderer").GetProperty("contents")[0].GetProperty("itemSectionRenderer").GetProperty("contents")[2].ToString();
            //string somePart = root.GetProperty("responseContext").GetProperty("contents").ToString();
            Console.WriteLine(somePart);
            File.WriteAllText(filePath, document.RootElement.ToString());

        } else {
            Console.WriteLine("Empty result");
        }

        //dynamic? jsonObject = JsonSerializer.Deserialize<dynamic>(divs.Last().InnerHtml);
        //dynamic title = jsonObject["title"];
        //Console.WriteLine(jsonObject);

        return "OK";
    }

    public static async Task<List<HtmlNode>> FetchDivsWithClassName(string url, string className)
{
    HttpClient httpClient = new HttpClient();
    List<HtmlNode> divs = new List<HtmlNode>();

    try
    {
        string html = await httpClient.GetStringAsync(url);

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);
        //Console.WriteLine(doc.Text);
        // Select the specific divs by class name
        //.Where(div => div.GetAttributeValue("class", "") == className)
        //divs = htmlDoc.DocumentNode.Descendants("ytd-video-renderer").ToList();
        //divs = doc.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("class", "").Contains("ytd-video-renderer")).ToList();
        divs = doc.DocumentNode.Descendants("script").OrderBy(x => x.InnerHtml.Length).ToList();
        
        //divs = doc.DocumentNode.SelectNodes("//div[@class='yt-lockup-content']").ToList();
        //divs.ForEach(div => Console.WriteLine(div.InnerHtml));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }

    return divs;
}

}