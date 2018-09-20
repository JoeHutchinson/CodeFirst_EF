using System.Data.Entity;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;

namespace CodeFirst_EF.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<CountVonCountDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "CodeFirst_EF.CountVonCountDBContext";
        }

        protected override void Seed(CountVonCountDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.WordMetrics.AddOrUpdate(new WordMetric("hashed1", "Vauxhall", 8));
            context.WordMetrics.AddOrUpdate(new WordMetric("hashed2", "Vauxhall", 5));
            context.WordMetrics.AddOrUpdate(new WordMetric("hashed3", "Bmw", 3));
        }
    }
}
