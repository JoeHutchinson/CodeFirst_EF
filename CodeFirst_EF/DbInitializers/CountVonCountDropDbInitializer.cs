using System.Data.Entity;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;

namespace CodeFirst_EF.DbInitializers
{
    public class CountVonCountDropDbInitializer : DropCreateDatabaseAlways<CountVonCountDbContext>
    {
        protected override void Seed(CountVonCountDbContext context)
        {
            // seed the DB
            context.WordMetrics.Add(new WordMetric("hashed1", "Vauxhall", 8));
            context.WordMetrics.Add(new WordMetric("hashed2", "Vauxhall", 5));
            context.WordMetrics.Add(new WordMetric("hashed3", "Bmw", 3));

            base.Seed(context);
        }
    }
}