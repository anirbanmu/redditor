using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;

public class RedditApi
{
    public static readonly Uri BaseUri = new Uri("https://www.reddit.com/");

    static T HttpRequest<T>(Uri uri)
    {
        var request = WebRequest.Create(uri) as HttpWebRequest;
        using (var response = request.GetResponse() as HttpWebResponse)
        {
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    return ParseJson<T>(result);
                }
            }
        }
    }

    static T ParseJson<T>(string jsonString)
    {
        return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public class ApiResponseContainer<T>
    {
        [JsonProperty("kind")]
        public string Kind { get; private set; }

        [JsonProperty("data")]
        public T Data { get; private set; }
    }

    public class UserInfo
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("created_utc")]
        [JsonConverter(typeof(TimestampToDateTimeConverter))]
        public DateTime AccountCreatedUtc { get; private set; }

        [JsonProperty("link_karma")]
        public int LinkKarma { get; private set; }

        [JsonProperty("comment_karma")]
        public int CommentKarma{ get; private set; }

        public class TimestampToDateTimeConverter : JsonConverter
        {
            public override bool CanConvert(Type t){ return t == typeof(DateTime); }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){ throw new NotImplementedException(); }
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64((double)reader.Value)).DateTime;
            }
        }
    }

    public static UserInfo GetUserInfo(string username)
    {
        try
        {
            Uri aboutUserUri = new Uri(BaseUri, "user/" + username + "/about.json");
            var json = HttpRequest<ApiResponseContainer<UserInfo>>(aboutUserUri);
            return json.Data;
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