using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TwitterManabiTool.Model
{
    public class LINEService
    {
        /// <summary>
        /// アクセストークンID
        /// </summary>
        private readonly string ACCESS_TOKEN = "cOIprzX222fnK5eHoGzpn4lqlpRrTngYDxDQK6QXYoE";

        public LINEService() 
        {
        }

        /// <summary>
        /// メッセージの送付
        /// </summary>
        /// <param name="message"></param>
        public async void PostMessage(string message)
        {
            using (var client = new HttpClient())
            {
                // 通知するメッセージ
                var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", message } });

                // ヘッダーにアクセストークンを追加
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.ACCESS_TOKEN);

                // 実行
                var result = await client.PostAsync("https://notify-api.line.me/api/notify", content);

                Console.WriteLine(result);
            }
        }

        /// <summary>
        /// トークン情報取得
        /// </summary>
        /// <param name="message"></param>
        public async Task<string> GetTokenInformation()
        {
            using (var client = new HttpClient())
            {
                // ヘッダーにアクセストークンを追加
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.ACCESS_TOKEN);

                // 実行
                var result = await client.GetAsync("https://notify-api.line.me/api/status");

                Console.WriteLine(result);

                return result.ToString();
            }
        }


        public void PostMessage()
        {
            var url = "https://notify-api.line.me/api/notify";
            var encode = Encoding.UTF8;
            var payload = "message=" + HttpUtility.UrlEncode("こんにちは!", encode);

            using (var wc = new WebClient())
            {
                wc.Encoding = encode;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                wc.Headers.Add("Authorization", "Bearer " + this.ACCESS_TOKEN);
                var response = wc.UploadString(url, payload);
            }
        }
    }
}
