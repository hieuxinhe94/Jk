using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ToolCore.Entities;

namespace ToolBusiness.Strategies
{
    public interface IStrategy
    {
        ICollection<Quiz> Scrapping();
        Task<ICollection<Quiz>> ScrappingAsync();
    }
}
