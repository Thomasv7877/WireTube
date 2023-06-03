using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

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
        foreach (HtmlNode div in divs)
        {
            // Do something with each div
            Console.WriteLine(div.InnerHtml);
        }
        } else {
            Console.WriteLine("Empty result");
        }

        return "OK";
    }

    public static async Task<List<HtmlNode>> FetchDivsWithClassName(string url, string className)
{
    HttpClient httpClient = new HttpClient();
    List<HtmlNode> divs = new List<HtmlNode>();

    try
    {
        string html = await httpClient.GetStringAsync(url);

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        Console.WriteLine(htmlDoc.Text);
        // Select the specific divs by class name
        //.Where(div => div.GetAttributeValue("class", "") == className)
        divs = htmlDoc.DocumentNode.Descendants("ytd-video-renderer")
            .ToList();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }

    return divs;
}

}