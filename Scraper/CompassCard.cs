using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace Scraper
{
    public class CompassCard
    {
        private const string url_frontpage = @"https://compasscard.511sd.com";
        private const string url_login = @"https://compasscard.511sd.com/webtix/welcome/welcome.do";
        private static HttpClient hc = new HttpClient();

        private readonly string password = ConfigurationManager.AppSettings["secretPassword"];
        private readonly List<string> cardNumbers = new List<string>(ConfigurationManager.AppSettings["cardNumberCsv"].Split(','));

        public CompassCard()
        {
        }

        public string GetFrontPage()
        {
            //var response = hc.PostAsync(url, new StringContent("login=XXX&password=YYY", Encoding.UTF8, ""));
            HttpResponseMessage response = hc.GetAsync(url_frontpage, HttpCompletionOption.ResponseContentRead).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public float LoginToSummaryPage()
        {
            var listOfKeyValuePairs = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("cardNum", cardNumbers[0]),
                new KeyValuePair<string, string>("pass", password),
                new KeyValuePair<string, string>("cardOps","Display"),
            };
            var response = hc.PostAsync(url_login, new FormUrlEncodedContent(listOfKeyValuePairs));
            response.Wait();
            var content = response.Result.Content.ReadAsStringAsync();
            content.Wait();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content.Result);

            float balance = float.Parse(htmlDoc.DocumentNode.SelectSingleNode("//table[contains(@class, 'results_table')][1]//td[2]").InnerText.Replace("$",""));

            return balance;
        }
    }
}
