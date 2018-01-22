using System;

namespace redditor
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = RedditApi.GetUserInfo("electrostat");
            Console.WriteLine(x);
        }
    }
}
