using System.Data.Entity;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.Migrations;

namespace CodeFirst_EF.DbInitializers
{
    internal class CountVonCountMigrateDbInitializer : MigrateDatabaseToLatestVersion<CountVonCountDbContext, Configuration>
    {
        
    }
}