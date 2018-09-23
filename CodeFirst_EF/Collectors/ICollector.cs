using System.Collections.Generic;
using CodeFirst_EF.DTOs;

namespace CodeFirst_EF.Collectors
{
    public interface ICollector
    {
        IEnumerable<WordMetric> Collect(string location);
    }
}
