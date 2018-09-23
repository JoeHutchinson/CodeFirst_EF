using CodeFirst_EF.DTOs;
using TddXt.AnyRoot.Numbers;
using TddXt.AnyRoot.Strings;
using static TddXt.AnyRoot.Root;

namespace CodeFirst_EF.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DbContexts.CountVonCountDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "CodeFirst_EF.DbContexts.CountVonCountDbContext";
        }

        protected override void Seed(DbContexts.CountVonCountDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            
            var word = Any.String(10);
            //context.WordMetrics.Add(new WordMetric(word, word, Any.Integer(), null));   //TODO: Remove these
            //context.TmpWordMetrics.Add(new TmpWordMetric(word, word, Any.Integer(), null));
        }
    }
}
