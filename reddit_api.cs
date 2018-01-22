using System;
using System.Net;
using System.IO;

public class RedditApi
{
    public static readonly Uri BaseUri = new Uri("https://www.reddit.com/");

    static string HttpRequest(Uri uri)
    {
        var request = (HttpWebRequest)WebRequest.Create(uri);
        using (var response = request.GetResponse() as HttpWebResponse)
        {
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }

    public static string GetUserInfo(string username)
    {
        try
        {
            Uri aboutUserUri = new Uri(BaseUri, "user/" + username + "/about.json");
            return HttpRequest(aboutUserUri);
        }
        catch(WebException e)
        {
            HttpStatusCode? statusCode = (e.Response as HttpWebResponse)?.StatusCode;
            if (statusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Reddit username not found.");
            }
            else Console.WriteLine("Error accessing Reddit API. [" + (statusCode != null ? statusCode.ToString() : e.Status.ToString()) + "]");
        }

        return "";
    }
}