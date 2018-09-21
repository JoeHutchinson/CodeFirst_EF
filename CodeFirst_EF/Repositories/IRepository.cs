using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CodeFirst_EF.Repositories
{
    /// <summary>
    /// Abstraction layer over the DB layer, allows other storage technology to be swapped in without effecting domain.
    /// Could define a Filter design pattern over the interface but felt we lose encapsulation as we'd expose DBSet.
    /// </summary>
    /// <typeparam name="T">Entity to save/get</typeparam>
    internal interface IRepository
    {
        IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? take = null) where TEntity : class, IEntity;

        void Upsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;
    }

    public interface IEntity
    {
        string Id { get; set; }
        string Word { get; set; }
        int Count { get; set; }
    }
}
