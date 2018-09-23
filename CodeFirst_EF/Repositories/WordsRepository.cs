using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CodeFirst_EF.Security;

namespace CodeFirst_EF.Repositories
{
    /// <summary>
    /// Wrapper over the EntityRepo it allows hiding of DB context so from a Controllers perspective
    /// you have no idea if I'm interacting with DB, Fileserver, web service etc.
    /// </summary>
    public sealed class WordsRepository : IWordRepository
    {
        private readonly IHashRepository _hashRepository;
        private readonly ISaltCache _saltCache;

        /// <summary>
        /// This can be cleaner if I create separate read and write repos
        /// </summary>
        /// <param name="saltCache"></param>
        /// <param name="hashRepository"></param>
        public WordsRepository(ISaltCache saltCache = null, IHashRepository hashRepository = null)
        {
            _hashRepository = hashRepository;
            _saltCache = saltCache;
        }

        public IEnumerable<WordMetric> GetTop(int take)
        {
            return GetTop<CountVonCountDbContext>(take);
        }

        public void Save(IEnumerable<WordMetric> words)
        {
            Save<CountVonCountDbContext>(words);
        }

        internal void Save<T>(IEnumerable<WordMetric> words) where T : DbContext, new()
        {
            using (var context = new T())
            {
                var efr = new EntityFrameworkRepository<T>(context, _hashRepository);

                _saltCache.Init(efr);

                efr.Upsert(words);

                context.SaveChanges();
            }
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
        void Save(IEnumerable<WordMetric> words);
    }
}
