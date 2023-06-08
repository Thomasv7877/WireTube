using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.Json;
using System.IO;
using System.Dynamic;

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
            //string somePart = root.GetProperty("contents").GetProperty("twoColumnSearchResultsRenderer").GetProperty("primaryContents").GetProperty("sectionListRenderer").GetProperty("contents")[0].GetProperty("itemSectionRenderer").GetProperty("contents")[2].ToString();
            IList<JsonElement> somePart = root.GetProperty("contents").GetProperty("twoColumnSearchResultsRenderer").GetProperty("primaryContents").GetProperty("sectionListRenderer").GetProperty("contents")[0].GetProperty("itemSectionRenderer").GetProperty("contents").EnumerateArray().Where(x => x.EnumerateObject().FirstOrDefault().Name == "videoRenderer").ToList();
            foreach(var vid in somePart){
                //dynamic dynVid = vid; // JsonElement to dynamic werkt niet..
                //string title = dynVid["videoRenderer"]["title"]["runs"][0]["text"];
                string vidId = vid.GetProperty("videoRenderer").GetProperty("videoId").ToString();
                string vidLink = "https://www.youtube.com/watch?v=" + vidId;
                string thumb = vid.GetProperty("videoRenderer").GetProperty("thumbnail").GetProperty("thumbnails")[0].GetProperty("url").ToString();
                string title = vid.GetProperty("videoRenderer").GetProperty("title").GetProperty("runs")[0].GetProperty("text").ToString();
                string channel = vid.GetProperty("videoRenderer").GetProperty("longBylineText").GetProperty("runs")[0].GetProperty("text").ToString();
                string length = vid.GetProperty("videoRenderer").GetProperty("lengthText").GetProperty("simpleText").ToString();
                int views = int.Parse(vid.GetProperty("videoRenderer").GetProperty("viewCountText").GetProperty("simpleText").ToString().Split(" ")[0].Replace(".", ""));

                //Console.WriteLine($"id: {vidId}, link: {vidLink}, thumb: {thumb.Substring(0, 20)}, title: {title}, channel: {channel}, length: {length}, views: {views}");

                // result.id.videoId
                // result.snippet.thumbnails.default.url
                // result.snippet.channelTitle
                // result.snippet.title
                // result.progress, result.downloading, result.views

                // workaround, 'default' als key gebruiken
                dynamic subObj = new ExpandoObject();
                subObj.@default = new { url = thumb };

                dynamic reactObj = new {
                    id = vidId,
                    snippet = new {
                        channelTitle = channel,
                        title = title,
                        thumbnails = new { @default = new { url = thumb } }
                    },
                    progress = 0,
                    downloading = false,
                    views = views
                };
                Console.WriteLine(reactObj.ToString());
            }
            //string somePart = root.GetProperty("responseContext").GetProperty("contents").ToString();
            //Console.WriteLine(somePart[0].ToString());
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
public async static Task<IEnumerable<dynamic>> searchYtAlt (string url){
    string ytRes = await ytSearch(url);
    IEnumerable<JsonElement> jsonList = parseYtToJson(ytRes);
    IEnumerable<dynamic> objList = jsonList.Select(x => convertJsonToDyn(x));
    return objList;
}

public async static Task searchYtTest (string url){
    string ytRes = await ytSearch(url);
    //Console.WriteLine(ytRes);
    //IEnumerable<JsonElement> jsonList = parseYtToJson(ytRes);
    //IEnumerable<dynamic> objList = jsonList.Select(x => convertJsonToDyn(x));
    //return null;
}

private static async Task<string> ytSearch(string url){
    HttpClient httpClient = new HttpClient();
    try
    {
        string html = await httpClient.GetStringAsync(url);
        Console.WriteLine(html.Substring(0, 50));

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);
        string ytRes = doc.DocumentNode.Descendants("script").OrderBy(x => x.InnerHtml.Length).Last().InnerHtml;
        return ytRes;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return null;
}
private static IEnumerable<JsonElement> parseYtToJson(string ytRes){
    int varLength = 20;
    JsonDocument document = JsonDocument.Parse(ytRes.Substring(varLength, ytRes.Length - (varLength + 1)));
    JsonElement root = document.RootElement;
    IEnumerable<JsonElement> jsonList = root.GetProperty("contents").GetProperty("twoColumnSearchResultsRenderer").GetProperty("primaryContents").GetProperty("sectionListRenderer").GetProperty("contents")[0].GetProperty("itemSectionRenderer").GetProperty("contents").EnumerateArray().Where(x => x.EnumerateObject().FirstOrDefault().Name == "videoRenderer").ToList();
    return jsonList;
}
private static dynamic convertJsonToDyn(JsonElement jsonElement){
    string vidId = jsonElement.GetProperty("videoRenderer").GetProperty("videoId").ToString();
    string vidLink = "https://www.youtube.com/watch?v=" + vidId;
    string thumb = jsonElement.GetProperty("videoRenderer").GetProperty("thumbnail").GetProperty("thumbnails")[0].GetProperty("url").ToString();
    string title = jsonElement.GetProperty("videoRenderer").GetProperty("title").GetProperty("runs")[0].GetProperty("text").ToString();
    string channel = jsonElement.GetProperty("videoRenderer").GetProperty("longBylineText").GetProperty("runs")[0].GetProperty("text").ToString();
    string length = jsonElement.GetProperty("videoRenderer").GetProperty("lengthText").GetProperty("simpleText").ToString();
    int views = int.Parse(jsonElement.GetProperty("videoRenderer").GetProperty("viewCountText").GetProperty("simpleText").ToString().Split(" ")[0].Replace(".", ""));

    dynamic reactObj = new {
        id = vidId,
        snippet = new {
            channelTitle = channel,
            title = title,
            thumbnails = new { @default = new { url = thumb } }
        },
        progress = 0,
        downloading = false,
        views = views
    };
    return reactObj;
}

}