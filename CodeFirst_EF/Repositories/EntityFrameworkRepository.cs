using CodeFirst_EF.Security;
using FastMember;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace CodeFirst_EF.Repositories
{
    /// <summary>
    /// Generic EF repository with a flexible Get method and performance optimised Upsert
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    internal sealed class EntityFrameworkRepository<TContext> : IRepository where TContext : DbContext, new()  //TODO: Separate this into Read and Write repos
    {
        private readonly TContext _context;
        private readonly IHashRepository _hashRepository;

        public EntityFrameworkRepository(TContext context, IHashRepository hashRepository = null)
        {
            _context = context;
            _hashRepository = hashRepository;
        }

        public void Upsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            Console.WriteLine("Hash start");
            entities = _hashRepository.Hash(entities);
            Console.WriteLine("Hash end");
            Console.WriteLine("Upsert start");
            Upsert($"Tmp{typeof(TEntity).Name}s", $"p_MergeInto{typeof(TEntity).Name}s", entities);
        }

        public IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? take = null) where TEntity : class, IEntity
        {
            
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.ToList();
        }

        internal void Upsert<TEntity>(string tableName, string mergeName, IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            using(var context = new TContext())
            using (var connection = context.Database.Connection as SqlConnection)
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                using (var reader = ObjectReader.Create(entities, typeof(TEntity).GetProperties().Select(p => p.Name).ToArray()))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(reader); //TODO: Add TVP
                    transaction.Commit();
                }
            }

            using (var context = new TContext())
            {
                context.Database.ExecuteSqlCommand($"exec {mergeName}");
                context.SaveChanges();

            }
        }
    }
}