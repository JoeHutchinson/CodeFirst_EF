using CodeFirst_EF.Repositories;
using CodeFirst_EF.Security;
using System.ComponentModel.DataAnnotations;

namespace CodeFirst_EF.DTOs
{
    public class TmpWordMetric : IEntity  //TODO: Could remove this
    {
        public TmpWordMetric() { }

        public TmpWordMetric(string id, string word, int count)
        {
            Id = id;
            Word = word;
            Count = count;
        }

        [Key, Hash]
        public string Id { get; set; }

        public int Count { get; set; }

        public string Word { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Word)}: {Word}, {nameof(Count)}: {Count}";
        }
    }
}