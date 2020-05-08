using Crawler.Business;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolCore.Entities;

namespace ToolBusiness.Strategies
{
    public class EnglishTestStoreStrategy : StrategyBase, IStrategy
    {

        public EnglishTestStoreStrategy()
        {
            this.CrawlerStrategy = CrawlerStrategy.EnglishTestStore;
            this.Title = "English Test Store Strategy";
            this.BaseUrl = "https://englishteststore.net/";
        }
        public ICollection<Quiz> Scrapping()
        {
            string targetSiteContent = CrawlerEngine.GetResponseString(BaseUrl + "/index.php?option=com_content&view=article&id=11387&Itemid=380").Result;
            var doc = new HtmlDocument();
            doc.Load(targetSiteContent);
            string url = "", section = "", subSection = "";

            var englishGrammarSectionNodes = doc.DocumentNode.SelectNodes("//div[@itemprop='articleBody']//table[1]//a[@href]");
            var englishGrammarSectionLinks = new List<string>();
            if (englishGrammarSectionNodes != null)
            {
                section = "Basic English Grammar";
                foreach (var node in englishGrammarSectionNodes)
                {
                    subSection = node.InnerText.Trim();
                    url = BaseUrl + node.GetAttributeValue("href", "");
                    englishGrammarSectionLinks.Add(url);

                    var subLinks = LookUpLinkAndScrapLinks(url, subSection);

                    foreach (var link in subLinks)
                    {
                        LookUpLinkAndScrapQuizes(link.Key, link.Value);
                    }
                }
            }

            //var englishTenseSectionLinks = new List<string>();
            //if (nodes != null)
            //{
            //    foreach (var node in nodes)
            //    {
            //        englishTenseSectionLinks.Add(node.GetAttributeValue("href", ""));
            //    }
            //}

            //var advEnglishGrammarSectionLinks = new List<string>();
            //if (nodes != null)
            //{
            //    foreach (var node in nodes)
            //    {
            //        advEnglishGrammarSectionLinks.Add(node.GetAttributeValue("href", ""));
            //    }
            //}

            //var commonlyConfusedSectionLinks = new List<string>();
            //if (nodes != null)
            //{
            //    foreach (var node in nodes)
            //    {
            //        commonlyConfusedSectionLinks.Add(node.GetAttributeValue("href", ""));
            //    }
            //}

            return null;
        }

        public Task<ICollection<Quiz>> ScrappingAsync()
        {
            throw new NotImplementedException();
        }


        private Dictionary<string, string> LookUpLinkAndScrapLinks(string url, string sectionName)
        {
            Dictionary<string, string> subLinks = new Dictionary<string, string>();
            string targetSiteContent = CrawlerEngine.GetResponseString(url).Result;
            var doc = new HtmlDocument();
            doc.Load(targetSiteContent);

            var nodes = doc.DocumentNode.SelectNodes($"//a[@href!=\"\"][contains(text(),'${sectionName}')]");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    subLinks.Add(node.GetAttributeValue("href", ""), node.InnerText);
                }
            }

            return subLinks;
        }

        private string LookUpLinkAndScrapQuizes(string url, string sectionName)
        {
            string targetSiteContent = CrawlerEngine.GetResponseString(url).Result;

            var iframeMatch = Regex.Match(targetSiteContent, "<iframe.+src=\\\"(?<1>.*?)\\\"");

            if (iframeMatch.Success)
            {
                url = BaseUrl + iframeMatch.Groups[1].Value;
                targetSiteContent = CrawlerEngine.GetResponseString(url).Result;

                var base64DataEncodedMatch = Regex.Match(targetSiteContent, "var data = \\\"(?<1>.*?)\\\";");

                if (base64DataEncodedMatch.Success)
                {
                    return CrawlerEngine.DecodeBase64String(base64DataEncodedMatch.Groups[1].Value);
                }
            }

            return null;
        }

        private ICollection<Quiz> ParseJsonToModel(string json)
        {
            return JsonConvert.DeserializeObject<List<Quiz>>(json);
        }

        //private async string LookUpLinkAndScrapAsync(string url, string sectionName)
        //{
        //    // string targetSiteContent = CrawlerEngine.GetResponseString(BaseUrl + "").Result;
        //    return "";
        //}
    }
}
