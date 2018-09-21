using System.Data.Entity;
using CodeFirst_EF.DbInitializers;
using CodeFirst_EF.DTOs;

namespace CodeFirst_EF.DbContexts
{
    public class CountVonCountDbContext : DbContext
    {
        public CountVonCountDbContext() : base("CountVonCountDBConnectionString")
        {
            Database.SetInitializer(new CountVonCountMigrateDbInitializer());
            //Database.SetInitializer(new CountVonCountDropDbInitializer());
        }

        protected CountVonCountDbContext(string connectionString) : base(connectionString) { }

        public DbSet<WordMetric> WordMetrics { get; set; }

        public DbSet<TmpWordMetric> TmpWordMetrics { get; set; }
    }
}