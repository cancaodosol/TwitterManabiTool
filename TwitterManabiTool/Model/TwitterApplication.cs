using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwitterManabiTool.Common;

namespace TwitterManabiTool.Model
{
    class TwitterApplication
    {
        private readonly TwitterService _twitterService;
        private readonly int _lowerLimitFavoriteCount = 3;
        public TwitterApplication()
        {
            _twitterService = new TwitterService();
        }

        /// <summary>
        /// 期間を指定し、いいね数ランキングをツイートします。
        /// </summary>
        /// <param name="period"></param>
        /// 過去何日分のツイートを取得するのか指定します。
        /// <param name="numOfRanking"></param>
        /// 何位までのランキングとするか指定します。
        public void TweetFavoritestRanking(int period, int numOfRanking)
        {
            var startDate = DateTime.Now.AddDays(-1 * period).ToString("yyyy/MM/dd");
            var endDate = DateTime.Now.ToString("MM/dd"); ;

            var topFavoriteTweetsRanking = this.GetFavoritestStatus(period, numOfRanking);

            foreach (var tweetsRanking in topFavoriteTweetsRanking)
            {
                var ranking = tweetsRanking.Ranking;
                var status = tweetsRanking.Status;
                var text = new StringBuilder();
                text.Append("【いいね数ランキング(" + startDate + "～" + endDate + ")】");
                text.Append(Environment.NewLine);
                text.Append("第" + ranking + "位：@" + status.User.ScreenName + " ☆ " + (int)status.FavoriteCount + " Goods☆");
                text.Append(Environment.NewLine);
                text.Append("――――――――――――――――――――");
                text.Append(Environment.NewLine);
                text.Append(TextHelper.LimitedChars(status.Text, 70));
                text.Append(Environment.NewLine);
                text.Append("[Tweeted by MANABI Tool]");
                text.Append(Environment.NewLine);
                Console.Write(text);
               _twitterService.Reply(text.ToString(),status.Id);
            }
        }

        private List<StatusRanking> GetFavoritestStatus(int period, int numOfRanking)
        {
            var lastStatus = _twitterService.GetStatusesLast(period);
            var sortedStatus = lastStatus.OrderByDescending(_ => _.FavoriteCount).ToList();

            var ranking = 0;
            var count = 1;
            var old = -1;
            var rankingList = new List<StatusRanking>();

            for (int i = 0; i < sortedStatus.Count; i++)
            {
                var top = sortedStatus[i];

                var favoriteCount = top.FavoriteCount != null ? (int)top.FavoriteCount : 0;
                if (favoriteCount < _lowerLimitFavoriteCount) break;
                if (old != favoriteCount) ranking = count;
                if (ranking > numOfRanking) break;

                var status = new StatusRanking(ranking, top);
                rankingList.Add(status);
                /*
                 * Console.WriteLine("【いいね数ランキング({0}～{1})】", startDate, endDate);
                Console.WriteLine("第{0}位：@{1} ☆ {2} Goods☆", ranking, top.User.ScreenName, favoriteCount);
                Console.WriteLine("――――――――――――――――――――");
                Console.WriteLine("{0}", TextHelper.LimitedChars(top.Text, 70));
                Console.WriteLine("");
                Console.WriteLine("[Tweeted by MANABI Tool]");
                */
                old = favoriteCount;
                count++;
            }

            return rankingList;
        }

        public void Test() 
        {
            var lastStatus = _twitterService.GetStatusesLast(7);
            var results = lastStatus.GroupBy(_ => _.User.ScreenName).Select(_ => new { Name = _.Key, TotalGoods = _.Sum(__ => __.FavoriteCount), TotalTweets = _.Count() });

            Console.WriteLine("       User       | Tweets | Favorites");
            Console.WriteLine("―――――――――――――――――――");
            foreach (var result in results)
            {
                Console.WriteLine("{0}|{1}   |{2}",TextHelper.MoveLeft(result.Name,18),TextHelper.MoveRight(result.TotalTweets.ToString(),6), TextHelper.MoveRight(result.TotalGoods.ToString(),7));
            }
        }
    }
    public class StatusRanking
    {
        public int Ranking;
        public CoreTweet.Status Status;
        public StatusRanking(int ranking,CoreTweet.Status status) 
        {
            Ranking = ranking;
            Status = status;
        }
    }
}
