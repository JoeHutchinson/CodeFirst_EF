using System.Collections;
using CodeFirst_EF.DTOs;
using System.Collections.Generic;

namespace CodeFirst_EF.Tests.Helpers
{
    internal class WordMetricComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var w1 = x as WordMetric;
            var w2 = y as WordMetric;

            if (w1.Word != w2.Word) return 1;
            if (w1.Id != w2.Id) return 1;
            if (w1.Count != w2.Count) return 1;
            return 0;
        }
    }
}
