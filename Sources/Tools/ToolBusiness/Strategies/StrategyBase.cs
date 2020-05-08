namespace ToolBusiness.Strategies
{
    public class StrategyBase
    {
        public string Guid = "C76F565F-B104-4142-89E0-29574A835979";
        public CrawlerStrategy CrawlerStrategy;
        public string BaseUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
