using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace TwitterManabiTool.Model
{
    public class TwitterService
    {
        private readonly string APIKey = "03cMf7MH6SuRuYSbpfHkZ29x8";
        private readonly string APISecret = "rQqylWxgIV5oRYrry5nQrQrPyE5hymU9gWlHlgjCqj7nqUstVM";
        private readonly string APIToken = "1265324744468951040-rdoy1GK16JvQar2GMt0xjuvNgNfvod";
        private readonly string APITokenSecret = "cNL77Tpb7GXkeYWAkLgyEtRlxU7cT5ZY2laTQ59VBek2C";
        private readonly string TokensUserScreenName = "manabi_matsui";

        public readonly Tokens Tokens;
        public TwitterService()
        {
            this.Tokens = CoreTweet.Tokens.Create(APIKey, APISecret, APIToken, APITokenSecret);
            this.Tokens.Statuses.HomeTimeline(tweet_mode => "extended");
        }

        public void Tweet(string text)
        {
            this.Tokens.Statuses.Update( status : text );
        }

        public void Reply(string text, long replyStateId) 
        {
            this.Tokens.Statuses.Update(
                status: text,
                in_reply_to_status_id: replyStateId
            );
        }

        public List<CoreTweet.Status> GetStatusesLast(int period) 
        {
            var statuses = new List<CoreTweet.Status>();
            var memberNames = this.Tokens.Friends.List().Select(_ => _.ScreenName).ToList();
            memberNames.Add(this.TokensUserScreenName);

            foreach (var name in memberNames)
            {
                var tweets = this.Tokens.Statuses.UserTimeline(count: 100, screen_name: name);
                foreach (var tweet in tweets)
                {
                    statuses.Add(tweet);
                    //Console.WriteLine("{0} : rep {1}: ret {2}: goods {3}: {4}",
                    //    tweet.CreatedAt.UtcDateTime,"no data",tweet.RetweetCount,tweet.FavoriteCount,tweet.Text);
                }
            }
            var lastStatuses = statuses.Where(_ => _.CreatedAt.UtcDateTime >= DateTime.Now.AddDays(-1 * period) && _.CreatedAt.UtcDateTime !=  DateTime.Now).ToList();

            return lastStatuses;
        }
    }
}
