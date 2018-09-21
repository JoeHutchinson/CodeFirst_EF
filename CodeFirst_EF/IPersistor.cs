using CodeFirst_EF.DTOs;
using System.Collections.Generic;

namespace CodeFirst_EF
{
    interface IPersistor
    {
        void Save(IEnumerable<WordMetric> entities);

        IEnumerable<WordMetric> Get(ICriteria criteria);
    }

    internal interface ICriteria
    {
        IEnumerable<WordMetric> meetCriteria();
    }
}
