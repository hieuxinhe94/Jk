using System;
using System.Collections.Generic;
using System.Text;
using ToolBusiness.Strategies;

namespace ToolBusiness
{
    public static class Tools
    {
        public static IStrategy ExtractStrategy(string strategy)
        {
            CrawlerStrategy crawlerStrategy;
            Enum.TryParse(strategy, out crawlerStrategy);

            switch (crawlerStrategy)
            {
                case CrawlerStrategy.EnglishTestStore:
                    return new EnglishTestStoreStrategy();
                case CrawlerStrategy.Other:

                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
