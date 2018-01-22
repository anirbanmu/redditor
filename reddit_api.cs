using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;

public class RedditApi
{
    public static readonly Uri BaseUri = new Uri("https://www.reddit.com/");

    static dynamic HttpRequest(Uri uri)
    {
        var request = WebRequest.Create(uri) as HttpWebRequest;
        using (var response = request.GetResponse() as HttpWebResponse)
        {
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    return ParseJson(result);
                }
            }
        }
    }

    static dynamic ParseJson(string jsonString)
    {
        return JsonConvert.DeserializeObject<dynamic>(jsonString);
    }

    public class UserInfo
    {
        public readonly string UserName;
        public readonly int LinkKarma;
        public readonly int CommentKarma;
        public readonly DateTime AccountCreated;

        public UserInfo(string name = "", int linkKarma = 0, int commentKarma = 0, int accountCreated = 0)
        {
            UserName = name;
            LinkKarma = linkKarma;
            CommentKarma = commentKarma;
            AccountCreated = DateTimeOffset.FromUnixTimeSeconds(accountCreated).DateTime;
        }
    }

    public static UserInfo GetUserInfo(string username)
    {
        try
        {
            Uri aboutUserUri = new Uri(BaseUri, "user/" + username + "/about.json");
            var json = HttpRequest(aboutUserUri).data;
            var user = new UserInfo(json.name.ToObject<string>(), json.link_karma.ToObject<int>(), json.comment_karma.ToObject<int>(), json.created_utc.ToObject<int>());
            return user;
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

        return null;
    }
}