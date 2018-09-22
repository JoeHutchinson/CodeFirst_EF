using System.Data.Entity;
using CodeFirst_EF.DbContexts;

namespace CodeFirst_EF.Tests.Helpers
{
    public class CountVonCountTestDbContext : CountVonCountDbContext
    {
        public CountVonCountTestDbContext() : base("CountVonCountTestDBConnectionString")
        {
            Database.SetInitializer(new CountVonCountMigrateTestDbInitializer());
        }
    }
}