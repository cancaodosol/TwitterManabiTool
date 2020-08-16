using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterManabiTool.Model;

namespace TwitterManabiTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var ser = new TwitterService();
            var app = new TwitterApplication();

            //app.Test();
            //app.TweetFavoritestRanking(7,5);

            var line = new LINEService();
            var str = Task.Run(() => line.GetTokenInformation());
            Console.WriteLine(str);
            line.PostMessage();
            line.PostMessage("Hello Line Message World !");
        }
    }
}
