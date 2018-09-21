using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using CodeFirst_EF.Security;
using TddXt.AnyRoot.Strings;
using static TddXt.AnyRoot.Root;

namespace CodeFirst_EF
{
    /// <summary>
    /// Models a persistence layer using EF Code First approach alongside Database Migrations in manual mode (the only way to support Always Enabled 
    /// with Code First approach). Additionally runs Entity Framework Extensions to provide performant bulk operations.
    /// 
    /// To enable migrations run the following from Package Manager Console
    /// enable-migrations
    /// 
    /// BulkCopy used to insert into Always Encrypted table.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Welcome");

            var pbkdf2Provider = new PBKDF2Provider();
            var hashResult = pbkdf2Provider.CreateHash("theseTimesTryMensSouls");
            Console.WriteLine(pbkdf2Provider.VerifyPassword("theseTimesTryMensSouls", hashResult.Hash, hashResult.Salt));

            Console.ReadLine();
        }

        private static void DBInteraction()
        {
            var connString = ConfigurationManager.ConnectionStrings["CountVonCountDBConnectionString"]
                .ConnectionString;

            // Initial read
            using (var context = new CountVonCountDbContext())
            {
                var persist = new EntityFrameworkRepository<CountVonCountDbContext>(connString, context);

                Console.WriteLine(@"Word Metrics");
                foreach (var result in persist.Get<WordMetric>())
                {
                    Console.WriteLine(result);
                }

                Console.WriteLine(@"Tmp Metrics");
                foreach (var wordMetric in persist.Get<TmpWordMetric>())
                {
                    Console.WriteLine(wordMetric);
                }
            }

            // Write
            using (var context = new CountVonCountDbContext())
            {
                var repository = new EntityFrameworkRepository<CountVonCountDbContext>(connString, context);
                repository.Upsert<WordMetric>(CreateWords(5));
                context.SaveChanges();
            }

            Console.WriteLine(@"Complete");

            // Assert
            using (var context = new CountVonCountDbContext())
            {
                var persist = new EntityFrameworkRepository<CountVonCountDbContext>(connString, context);

                Expression<Func<WordMetric, bool>> where = wordM => wordM.Count > 10;
                IOrderedQueryable<WordMetric> OrderBy(IQueryable<WordMetric> entities) => entities.OrderByDescending(x => x.Count);
                const int take = 100;

                var results = persist.Get(@where, OrderBy, take);

                foreach (var result in results)
                {
                    Console.WriteLine($@"repo : {result}");
                }
            }
        }

        public static IEnumerable<WordMetric> CreateWords(int num)
        {
            for (var i = 0; i < num; i++)
            {
                var word = Any.String(10);
                yield return new WordMetric(word, word, 3);
            }
        }
    }
}
