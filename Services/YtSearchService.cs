using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.Json;
using System.IO;
using System.Dynamic;

namespace WebApi.Services;

public static class YtSearchService {

// logic split in 3 helper functions: do http request (HtmlAgilityPack) > convert to json > convert to object for returning
public async static Task<IEnumerable<dynamic>> searchYtAlt (string url){
    string ytRes = await ytSearch(url);
    IEnumerable<JsonElement> jsonList = parseYtToJson(ytRes);
    IEnumerable<dynamic> objList = jsonList.Select(x => convertJsonToDyn(x));
    return objList;
}
// do http request wit HtmlAgilityPack, filtering on HtmlDocument is needed, as search results reside within the largest script element
private static async Task<string> ytSearch(string url){
    HttpClient httpClient = new HttpClient();
    try
    {
        string html = await httpClient.GetStringAsync(url);
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
// convert html doc to json, then traverse the json and get a list of properties whose children contain the needed video info
// REM! if yt search without api breaks the logic of the jsonList variable needs to be updated
private static IEnumerable<JsonElement> parseYtToJson(string ytRes){
    int varLength = 20;
    JsonDocument document = JsonDocument.Parse(ytRes.Substring(varLength, ytRes.Length - (varLength + 1)));
    JsonElement root = document.RootElement;
    // deepest 'contents' property has children with video info, but only those with the name videoRenderer, unneeded results: ListRenderer (related vids), Channelrenderer
    // Tip: EnumerateArray and EnumerateObject help with complex json traversal
    IEnumerable<JsonElement> jsonList = root
        .GetProperty("contents")
        .GetProperty("twoColumnSearchResultsRenderer")
        .GetProperty("primaryContents")
        .GetProperty("sectionListRenderer")
        .GetProperty("contents")[0]
        .GetProperty("itemSectionRenderer")
        .GetProperty("contents").EnumerateArray().Where(x => x.EnumerateObject().FirstOrDefault().Name == "videoRenderer")
        .ToList();
    return jsonList;
}
// generate a result object for each 'videoRenderer' json part, needed: vid id, vid link, thumbnail, title, channel, vid length, views
private static dynamic convertJsonToDyn(JsonElement jsonElement){
    string vidId = jsonElement.GetProperty("videoRenderer").GetProperty("videoId").ToString();
    string vidLink = "https://www.youtube.com/watch?v=" + vidId;
    string thumb = jsonElement.GetProperty("videoRenderer").GetProperty("thumbnail").GetProperty("thumbnails")[0].GetProperty("url").ToString();
    string title = jsonElement.GetProperty("videoRenderer").GetProperty("title").GetProperty("runs")[0].GetProperty("text").ToString();
    string channel = jsonElement.GetProperty("videoRenderer").GetProperty("longBylineText").GetProperty("runs")[0].GetProperty("text").ToString();
    string length = jsonElement.GetProperty("videoRenderer").GetProperty("lengthText").GetProperty("simpleText").ToString();
    int views = int.Parse(jsonElement.GetProperty("videoRenderer").GetProperty("viewCountText").GetProperty("simpleText").ToString().Split(" ")[0].Replace(".", ""));

    // REM: in case of reserved keys like 'default' use @ prefix: ex '@default', no longer needed here

    dynamic reactObj = new {
        id = new { videoId = vidId},
        snippet = new {
            channelTitle = channel,
            title = title,
            thumbnails = new {medium = new { url = thumb } }
        },
        progress = 0,
        downloading = false,
        views = views
    };
    return reactObj;
}

}