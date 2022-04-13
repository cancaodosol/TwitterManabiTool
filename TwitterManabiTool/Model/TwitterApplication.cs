using CoreTweet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
                _twitterService.Reply(text.ToString(), status.Id);
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

        public void GetMostFavoritedTweets(string saerchMaxCount)
        {
            var targetUsers = new string[] { 
                "curiosity_com",
                "yuki_99_s",
                "wakamusya_eigyo",
                "sota_tominaga",
                "mikorino"
            };
            var tweets = _twitterService.GetTweetsByUserScreenNames(10, targetUsers);

            // 誰かへの返信リツイートは除去
            tweets = tweets.Where(x => string.IsNullOrEmpty(x.InReplyToScreenName)).ToList();

            Console.WriteLine("========= いいね多い順 =========");
            tweets = tweets.OrderByDescending(x => x.FavoriteCount).ToList();
            this.DisplayTweets(tweets);

            Console.WriteLine("========= リツイート多い順 =========");
            tweets = tweets.OrderByDescending(x => x.RetweetCount).ToList();
            this.DisplayTweets(tweets);
        }

        /// <summary>
        /// 特定のツイートにいいねしたユーザーのプロフィールバナーを取得します。
        /// </summary>
        /// <param name="targetTweetID"></param>
        /// <param name="searchResultMax"></param>
        public void GetProfileBannersByTweetID(string targetTweetID, int searchResultMax)
        {
            Console.WriteLine("準備中です。");
        }

        /// <summary>
        /// 特定のユーザーのフォロワーのプロフィールバナーを取得します。
        /// </summary>
        /// <param name="targetTweetID"></param>
        /// <param name="searchResultMax"></param>
        public void GetProfileBannersByUserID(string targetUserID, int searchResultMax)
        {
            Console.WriteLine("準備中です。");
        }

        /// <summary>
        /// 自分のフォロワーのプロフィールバナーを取得します。
        /// </summary>
        public void GetProfileBannersByMyFriends()
        {
            var myFriends = _twitterService.GetMyFriends();
            this.SaveUserProfileBanner(myFriends, "myfriends");
        }

        /// <summary>
        /// プロフィールバナーを検索キーワードから取得します。
        /// </summary>
        /// <param name="searchKeyWord"></param>
        /// <param name="searchResultMax"></param>
        public void GetProfileBannersBySearchKeyWord(string searchKeyWord, int searchResultMax)
        {
            /*
            this.DisplayTweets(_twitterService.GetTweetsOfTargetUser("memoer15", 5));
            this.DisplayTweets(_twitterService.GetTweetsOfFrinds(5));
            this.DisplayTweets(_twitterService.GetSearchTweets("Airbnbが熱すぎる！", 10));
            */
            // this.DisplayTweets(_twitterService.GetTweetsOfTargetUser("memoer15", 5));
            var states = _twitterService.GetSearchTweets(searchKeyWord, searchResultMax);
            this.DisplayTweets(states);
            var users = states.Select(x => x.User).ToList();
            this.SaveUserProfileBanner(users, searchKeyWord);
        }
        public void SaveUserProfileBanner(List<User> users, string fileNameKeyWord)
        {
            var resultImageWidth = 600;
            var resultImageHeight = 600;
            var profileBannerImageWidth = 600;
            var profileBannerImageHeight = 200;
            var profileIconImageWidth = 135;
            var profileBannerIconHeight = 135;

            // ユーザーの重複を除去
            users = users.Distinct().ToList();

            // プロフィール画像、バナー画像が登録されてないユーザーは、除去
            users = users.Where(x => !string.IsNullOrEmpty(x.ProfileBannerUrl) && !string.IsNullOrEmpty(x.ProfileImageUrl)).ToList();

            // フォロワー数が多い順にソート
            users = users.OrderByDescending(x => x.FollowersCount).ToList();

            // Twitterユーザーのプロフィール一覧を作成する。
            var resultImage = new Bitmap(resultImageWidth, resultImageHeight * users.Count);
            using (var g = Graphics.FromImage(resultImage))
            {
                foreach (var (user, index) in users.Select((user , index) => (user, index)))
                {
                    var imageTopHeight = resultImageHeight * index;
                    Console.WriteLine("User : {0} [{1}/{2}], ProfileBannerUrl : {3}", user.Name, user.FriendsCount, user.FollowersCount, user.ProfileBannerUrl);
                    var bannerImage = LoadImageFromUrl(user.ProfileBannerUrl, profileBannerImageWidth, profileBannerImageHeight);
                    var profileImage = LoadImageFromUrl(user.ProfileImageUrl, profileIconImageWidth, profileBannerIconHeight);
                    if (bannerImage != null && profileImage != null)
                    {
                        var topMargin = 50;
                        // Twitterユーザー名を表示
                        g.DrawString(user.Name, new Font("Meiryo", 15), Brushes.Black, new Point(10, imageTopHeight+ 20));

                        // プロフィール画像と、バナーを表示
                        g.DrawImage(bannerImage, new Point(0, imageTopHeight + topMargin));
                        g.DrawImage(profileImage, new Point(40, imageTopHeight + topMargin + bannerImage.Height - profileImage.Height / 2));

                        // TwitterユーザーIDをプロフィール画像の下に表示
                        g.DrawString(user.Name, new Font("Meiryo", 15), Brushes.Black, new Point(20, imageTopHeight + topMargin + bannerImage.Height + profileImage.Height / 2 + 20));
                        g.DrawString("@" + user.ScreenName, new Font("Meiryo", 15), Brushes.Black, new Point(20, imageTopHeight + topMargin + bannerImage.Height + profileImage.Height / 2 + 50));

                        // プロフィールの説明文をバナー下に表示
                        var descriptionBox = new RectangleF(20, imageTopHeight + topMargin + bannerImage.Height + profileImage.Height / 2 + 80, resultImageWidth - 32, 270);
                        g.DrawString(user.Description, new Font("Meiryo", 12), Brushes.Black, descriptionBox);

                        // Twitterユーザーのフォロワー数、フォロー数をプロフィールの説明文の下に表示
                        g.DrawString("[" + user.FriendsCount + " フォロー中  " + user.FollowersCount + " フォロワー ]", new Font("Meiryo", 12), Brushes.Black, new Point(20, imageTopHeight + resultImageHeight - 30));    
                    }
                }
            }
            resultImage.Save("C:\\headless\\" + fileNameKeyWord + "_" + users.Count + ".bmp");
            resultImage.Dispose();
        }

        /// <summary>
        /// ツイート情報をコンソールに一覧出力します。
        /// </summary>
        /// <param name="tweets"></param>
        private void DisplayTweets(List<Status> tweets)
        {
            foreach (var tweet in tweets)
            {
                Console.WriteLine("Id : {0}", tweet.Id);
                Console.WriteLine("CreatedAt : {0}", tweet.CreatedAt);
                Console.WriteLine("User : {0} (@{1}) {2} / {3}", tweet.User.Name, tweet.User.ScreenName, tweet.User.FollowersCount, tweet.User.FriendsCount);
                Console.WriteLine("Text : {0}", tweet.Text);
                Console.WriteLine("FavoriteCount : {0}", tweet.FavoriteCount);
                Console.WriteLine("RetweetCount : {0}", tweet.RetweetCount);
                Console.WriteLine("--------------------------------");
            }
        }

        public void Test2()
        {
            var lastStatus = _twitterService.GetStatusesLast(7);
            var results = lastStatus.GroupBy(_ => _.User.ScreenName).Select(_ => new { Name = _.Key, TotalGoods = _.Sum(__ => __.FavoriteCount), TotalTweets = _.Count() });

            Console.WriteLine("       User       | Tweets | Favorites");
            Console.WriteLine("―――――――――――――――――――");
            foreach (var result in results)
            {
                Console.WriteLine("{0}|{1}   |{2}", TextHelper.MoveLeft(result.Name, 18), TextHelper.MoveRight(result.TotalTweets.ToString(), 6), TextHelper.MoveRight(result.TotalGoods.ToString(), 7));
            }
        }

        public void MakeTextFileAllTweets()
        {
            var tweets = _twitterService.GetStatusesLast(15).Where(t => !t.Text.Contains("Tweeted by MANABI Tool") && !t.Text.Contains("Goods☆")).ToList();
            var outputText = new StringBuilder();

            tweets.ForEach(t => outputText.Append(t.Text));
            File.AppendAllText(@"C:\Users\Hideyuki MATSUI\Desktop\Matsui\Tools\TwitterManabiTool\outputtext\alltweet" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt", outputText.ToString());

            return;
        }

        /// <summary>
        /// 新規ユーザーの利用承認確認用のWebページを開きます。
        /// </summary>
        public void OpenAuthorizeUserPage() 
        {
            var authorizeUri = _twitterService.GetUserAuthorizeUri();
            var pi = new ProcessStartInfo()
            {
                FileName = authorizeUri,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(pi);
        }
        public void SetTokensByPincode(string pincode) 
        {
            _twitterService.SetTokensByPincode(pincode);
        }

        /// <summary>
        /// URL指定で、画像ファイルを取得します。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Image LoadImageFromUrl(string url)
        {
            const int buffSize = 65536;
            var imgStream = new MemoryStream();

            if (string.IsNullOrWhiteSpace(url)) {
                return null;
            }

            try
            {
                var req = WebRequest.Create(url);
                var reader = new BinaryReader(req.GetResponse().GetResponseStream());

                while (true)
                {
                    var buff = new byte[buffSize];
                    int readBytes = reader.Read(buff, 0, buffSize);
                    if (readBytes <= 0)
                    {
                        break;
                    }
                    imgStream.Write(buff, 0, readBytes);
                }

                return new Bitmap(imgStream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error URL : {0}", url);
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public Image LoadImageFromUrl(string url, int imageWidth)
        {
            var image = this.LoadImageFromUrl(url);
            if (image == null) return null;
            var imageBitmap = new Bitmap(image);
            return (Image)(new Bitmap(imageBitmap, new Size(imageWidth, imageBitmap.Height * imageWidth / imageBitmap.Width)));
        }
        public Image LoadImageFromUrl(string url, int imageWidth, int imageHeight)
        {
            var image = this.LoadImageFromUrl(url);
            if (image == null) return null;
            var imageBitmap = new Bitmap(image);
            return (Image)(new Bitmap(imageBitmap, new Size(imageWidth, imageHeight)));
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
