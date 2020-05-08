using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ToolBusiness;

namespace Crawler.Business
{
    public class CrawlerEngine
    {
        public static async Task<string> GetResponseString(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var contents = await response.Content.ReadAsStringAsync();

                return contents;
            }
        }


        public static string DecodeBase64String(string content)
        {
            byte[] data = System.Convert.FromBase64String(content);
            return System.Text.ASCIIEncoding.ASCII.GetString(data);
        }

        public static ICollection<string> ExtractReferLinks(string content)
        {
            string xpath = "//a[@href !='']";

            var doc = new HtmlDocument();
            doc.Load(content);

            var nodes = doc.DocumentNode.SelectNodes(xpath);

            return nodes.Select(t => t.InnerText).ToList();
        }
    }
}
