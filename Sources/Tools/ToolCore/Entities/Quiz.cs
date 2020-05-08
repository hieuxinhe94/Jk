using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ToolCore.Entities
{
    public class BaseInfo
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        public string Desc { get; set; }
    
        public string Source { get; set; }
        public QuizStatus Status { get; set; }
    }

    public class Quiz: BaseInfo
    {
        public ICollection<Choice> Choices { get; set; }
    }

    public class QuizGroup: BaseInfo
    {
        public ICollection<Quiz> Quizes { get; set; }
    }

    public class QuizGroupSection : BaseInfo
    {
        public ICollection<QuizGroup> QuizGroups { get; set; }
    }

    public class QuizModule : BaseInfo
    {
        public ICollection<QuizGroupSection> QuizGroupSections { get; set; }
    }

    public class QuizModuleGroup : BaseInfo
    {
        public ICollection<QuizModule> QuizModules { get; set; }
    }


    public class Choice
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string Desc { get; set; }
        public bool IsCorrect { get; set; }
    }

}
