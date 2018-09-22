using System.Data.Entity;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Migrations;

namespace CodeFirst_EF.Tests.Helpers
{
    internal class CountVonCountMigrateTestDbInitializer : MigrateDatabaseToLatestVersion<CountVonCountDbContext, Configuration>
    {
        public override void InitializeDatabase(CountVonCountDbContext context)
        {

            base.InitializeDatabase(context);
        }
    }
}