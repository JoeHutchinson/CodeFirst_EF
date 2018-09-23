using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CodeFirst_EF.Repositories
{
    public sealed class WordsRepository : IWordRepository
    {
        public IEnumerable<WordMetric> GetTop(int take)
        {
            return GetTop<CountVonCountDbContext>(take);
        }

        internal IEnumerable<WordMetric> GetTop<T>(int take) where T : DbContext, new()
        {
            using (var context = new T())
            {
                var efr = new EntityFrameworkRepository<T>(context);

                IOrderedQueryable<WordMetric> OrderBy(IQueryable<WordMetric> words) => words.OrderByDescending(x => x.Count);

                var entities = efr.Get<WordMetric>(null, OrderBy, take);

                return entities;
            }
        }
    }

    public interface IWordRepository
    {
        IEnumerable<WordMetric> GetTop(int take);
    }
}
