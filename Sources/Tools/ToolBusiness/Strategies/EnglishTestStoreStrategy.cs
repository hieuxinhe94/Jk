using Crawler.Business;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolCore;
using ToolCore.Entities;

namespace ToolBusiness.Strategies
{
    public class EnglishTestStoreStrategy : StrategyBase, IStrategy
    {

        public EnglishTestStoreStrategy()
        {
            this.CrawlerStrategy = CrawlerStrategy.EnglishTestStore;
            this.Title = "English Test Store Strategy";
            this.BaseUrl = "https://englishteststore.net";
        }
        public ICollection<QuizModuleGroup> Scrapping()
        {
            List<QuizModuleGroup> moduleGroups = new List<QuizModuleGroup>();

            QuizModuleGroup moduleGroup = new QuizModuleGroup();
            moduleGroup.Title = "EnglishGrammar";
            moduleGroup.Source = "EnglishTestStore";
            moduleGroup.Status = QuizStatus.JustCreated;

            string targetSiteContent = CrawlerEngine.GetResponseString(BaseUrl + "/index.php?option=com_content&view=article&id=11387&Itemid=380");
            var doc = new HtmlDocument();
            doc.LoadHtml(targetSiteContent);
            string url = "", subSection = "";
            moduleGroup.QuizModules = new List<QuizModule>();

            var englishGrammarSectionNodes = doc.DocumentNode.SelectNodes("//div[@itemprop='articleBody']//table[1]//li/a[@href]");

            if (englishGrammarSectionNodes != null)
            {
                QuizModule module = new QuizModule();
                module.Title = "Basic English Grammar exercises and tests";
                module.Source = "EnglishTestStore";
                module.Status = QuizStatus.JustCreated;
                module.QuizGroupSections = new List<QuizGroupSection>();

                foreach (var node in englishGrammarSectionNodes.Take(1))
                {
                    QuizGroupSection section = new QuizGroupSection();
                    section.Title = node.InnerText.Trim();
                    section.Status = QuizStatus.JustCreated;
                    section.QuizGroups = new List<QuizGroup>();

                    url = BaseUrl + node.GetAttributeValue("href", "");
                    var subLinks = LookUpLinkAndScrapLinks(url, section.Title);

                    foreach (var link in subLinks)
                    {
                        QuizGroup group = new QuizGroup();
                        group.Title = node.InnerText.Trim();
                        string json = LookUpLinkAndScrapQuizes(link.Key, link.Value);
                        group.Quizes = ParseJsonToModel(json);

                        section.QuizGroups.Add(group);
                    }

                    module.QuizGroupSections.Add(section);
                }

                moduleGroup.QuizModules.Add(module);
            }

            moduleGroups.Add(moduleGroup);

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

            return moduleGroups;
        }

        public Task<ICollection<Quiz>> ScrappingAsync()
        {
            throw new NotImplementedException();
        }


        private Dictionary<string, string> LookUpLinkAndScrapLinks(string url, string sectionName)
        {
            Dictionary<string, string> subLinks = new Dictionary<string, string>();
            string targetSiteContent = CrawlerEngine.GetResponseString(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(targetSiteContent);

            var nodes = doc.DocumentNode.SelectNodes($"//a[@href!=\"\"][contains(text(),'{sectionName}')]");

            if(nodes == null)
            {
                sectionName = sectionName.Split(' ')[0];
                nodes = doc.DocumentNode.SelectNodes($"//a[@href!=\"\"][contains(text(),'{sectionName}')]");
            }

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    try
                    {
                        subLinks.Add(node.GetAttributeValue("href", ""), node.InnerText);
                    }
                    catch  
                    {
                    }
                }
            }

            return subLinks;
        }

        private string LookUpLinkAndScrapQuizes(string url, string sectionName)
        {
            string targetSiteContent = CrawlerEngine.GetResponseString(BaseUrl + url);

            var iframeMatch = Regex.Match(targetSiteContent, "<iframe.+src=\\\"(?<1>.*?)\\\"");

            if (iframeMatch.Success)
            {
                url = BaseUrl + iframeMatch.Groups[1].Value;
                targetSiteContent = CrawlerEngine.GetResponseString(url);

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
            List<Quiz> result = new List<Quiz>();
            JObject obj = JsonConvert.DeserializeObject<JObject>(json);
            var jQuizes = obj.SelectToken("d.sl.g[1].S");

            if (jQuizes == null) return null;

            foreach (var jquiz in jQuizes)
            {
                Quiz quiz = new Quiz();
                var jQuestion = jquiz.SelectToken("D.d[0]");
                if (jQuestion == null) break;

                quiz.Title = jQuestion.ToString();
                quiz.Desc = "";
                quiz.Status = QuizStatus.JustCreated;

                var jChoices = jquiz.SelectToken("C.chs");
                if (jChoices == null) break;

                quiz.Choices = new List<Choice>();
                foreach (var jChoice in jChoices)
                {
                    quiz.Choices.Add(new Choice
                    {
                        Text = jChoice.SelectToken("t.d[0]").ToString(),
                        Desc = "",
                        IsCorrect = (jChoice.SelectToken("c").ToString().ToLower() == "true")
                    });
                }

                result.Add(quiz);
            }

            return result;
        }

        //private async string LookUpLinkAndScrapAsync(string url, string sectionName)
        //{
        //    // string targetSiteContent = CrawlerEngine.GetResponseString(BaseUrl + "").Result;
        //    return "";
        //}
    }
}
