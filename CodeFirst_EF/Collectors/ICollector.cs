using System.Collections.Generic;

namespace CodeFirst_EF.Collectors
{
    public interface ICollector
    {
        IEnumerable<KeyValuePair<string, int>> CollectWords(string location);
    }
}
