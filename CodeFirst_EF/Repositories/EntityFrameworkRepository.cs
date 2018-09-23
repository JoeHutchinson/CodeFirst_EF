using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.Security;
using FastMember;

namespace CodeFirst_EF.Repositories
{
    /// <summary>
    /// Generic EF repository with a flexible Get method and performance optimised Upsert
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    internal class EntityFrameworkRepository<TContext> : IRepository where TContext : DbContext, new()
    {
        private readonly TContext _context;

        public EntityFrameworkRepository(TContext context, IHashProvider hashProvider = null)   //TODO: Probably want to start hashing
        {
            _context = context;
        }

        public void Upsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
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
            using (var connection =
                _context.Database.Connection as SqlConnection)
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                using (var reader = ObjectReader.Create(entities, typeof(TEntity).GetProperties().Select(p => p.Name).ToArray()))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(reader);
                    transaction.Commit();
                }
                    //TODO: Add TVP
                using (var context = new CountVonCountDbContext())  //TODO: Use above transaction?
                {
                    context.Database.ExecuteSqlCommand($"exec {mergeName}");
                    context.SaveChanges();
                }
            }
        }
    }
}