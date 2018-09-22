using System.Data.Entity;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.Tests.Helpers;
using NUnit.Framework;

namespace CodeFirst_EF.Tests.IntegrationTests
{
    public abstract class IntegrationTestSetup
    {

        public virtual void Init()
        {
            InitDb();
            // Do bootstrapping activities here
        }

        private static void InitDb()
        {
            using (var context = new CountVonCountTestDbContext())
            {
                // Force DB initialisation using migration scripts
                context.Database.Initialize(true);
                ClearDb(context);

                context.SaveChanges();
            }
        }

        private static void ClearDb(CountVonCountDbContext context)
        {
            context.WordMetrics.RemoveRange(context.WordMetrics);
            context.TmpWordMetrics.RemoveRange(context.TmpWordMetrics);
        }
    }
}