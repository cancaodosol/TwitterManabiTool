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

            /*
            var line = new LINEService();
            var str = Task.Run(() => line.GetTokenInformation());
            Console.WriteLine(str);
            line.PostMessage();
            line.PostMessage("Hello Line Message World !");
            */

            var isContinued = true;

            while (isContinued)
            {
                Console.WriteLine("======== Twitterサービスメニュー ========");
                foreach (var menu in TwitterServiceMenus) 
                {
                    Console.WriteLine(menu);
                }
                Console.WriteLine("実行するメニューを選択してください。");
                var menuNo = Console.ReadLine();

                var isMenuContinued = true;
                while (isMenuContinued)
                {
                    isMenuContinued = false;
                    switch (menuNo)
                    {
                        case "1":
                            { 
                                Console.WriteLine("検索したいキーワードを入力してください。");
                                var saerchKeyWord = Console.ReadLine();
                                Console.WriteLine("最大検索数を指定してください。（Max：50）");
                                var saerchMaxCount = Console.ReadLine();
                                app.GetProfileBannersBySearchKeyWord(saerchKeyWord, int.Parse(saerchMaxCount));
                                break;
                            }
                        case "2":
                            {
                                Console.WriteLine("検索対象のツイートIDを入力してください。");
                                var targetTweetID = Console.ReadLine();
                                Console.WriteLine("最大検索数を指定してください。（Max：50）");
                                var saerchMaxCount = Console.ReadLine();
                                app.GetProfileBannersByTweetID(targetTweetID, int.Parse(saerchMaxCount));
                                break;
                            }
                        case "3":
                            {
                                Console.WriteLine("検索対象のユーザーIDを入力してください。");
                                var targetUserID = Console.ReadLine();
                                Console.WriteLine("最大検索数を指定してください。（Max：50）");
                                var saerchMaxCount = Console.ReadLine();
                                app.GetProfileBannersByUserID(targetUserID, int.Parse(saerchMaxCount));
                                break;
                            }
                        case "4":
                            {
                                Console.WriteLine("最大検索数を指定してください。（Max：50）");
                                var saerchMaxCount = Console.ReadLine();
                                app.GetProfileBannersByMyFriends();
                                break;
                            }
                        case "10":
                            {
                                Console.WriteLine("最大検索数を指定してください。（Max：50）");
                                var saerchMaxCount = Console.ReadLine();
                                app.GetMostFavoritedTweets(saerchMaxCount);
                                break;
                            }
                        case "81":
                            {
                                app.OpenAuthorizeUserPage();
                                Console.WriteLine("承認後に画面に表示される8桁のPINコードを入力してください。");
                                var pincode = Console.ReadLine();
                                app.SetTokensByPincode(pincode);
                                break;
                            }
                        case "82":
                            {
                                Console.WriteLine("8桁のPINコードを入力してください。");
                                var pincode = Console.ReadLine();
                                app.SetTokensByPincode(pincode);
                                break;
                            }
                        case "99":
                            isContinued = false;
                            break;
                        default:
                            Console.WriteLine("該当のメニューNoがありません。再度入力してください。");
                            break;
                    }
                    if (isContinued)
                    {
                        Console.WriteLine("もう一度続けて同じメニューを行いますか？[Y/N]");
                        isMenuContinued = (new string[] { "Y", "y", "Ｙ", "ｙ" }).Contains(Console.ReadLine());
                    }
                }
            }

            //app.MakeTextFileAllTweets();
        }

        private static List<TwitterServiceMenu> TwitterServiceMenus = new List<TwitterServiceMenu>() {
            new TwitterServiceMenu(1, "プロフィールバーナーの取得[キーワード指定]", ""),
            new TwitterServiceMenu(2, "プロフィールバーナーの取得[ツイート指定]", ""),
            new TwitterServiceMenu(3, "プロフィールバーナーの取得[ユーザー指定]", ""),
            new TwitterServiceMenu(4, "プロフィールバーナーの取得[自分のフォロワー]", ""),
            new TwitterServiceMenu(10, "いいねの多いツイートの取得[ユーザー固定]", ""),
            new TwitterServiceMenu(81, "新しいユーザートークンを作成します。", ""),
            new TwitterServiceMenu(82, "PINコードを入力して、新しいユーザートークンを作成します。", ""),
            new TwitterServiceMenu(99, "プログラムを終了する", "")
        };

        private class TwitterServiceMenu {
            public int MenuNo;
            public string MenuName;
            public string Description;
            public TwitterServiceMenu(int menuNo, string menuName, string description) 
            {
                this.MenuNo = menuNo;
                this.MenuName = menuName;
                this.Description = description;
            }
            public override string ToString() {
                return "[" + this.MenuNo + "] : " + this.MenuName + " / " + this.Description; 
            }
        }
    }
}
