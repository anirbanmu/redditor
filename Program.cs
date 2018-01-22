using System;

namespace redditor
{
    class Program
    {
        static void Main(string[] args)
        {
            var userInfo = RedditApi.GetUserInfo(args[0]);
            Console.WriteLine("Username: " + userInfo.Name);
            Console.WriteLine("Link karma: " + userInfo.LinkKarma);
            Console.WriteLine("Comment karma: " + userInfo.CommentKarma);
            Console.WriteLine("Account created: " + userInfo.AccountCreatedUtc + " UTC");
        }

    }
}
