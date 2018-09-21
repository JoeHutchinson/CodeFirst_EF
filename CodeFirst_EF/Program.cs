using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Persistence;
using FastMember;
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

            using (var context = new CountVonCountDbContext())
            {
                Console.WriteLine(@"Word Metrics");
                foreach (var wordMetric in context.WordMetrics)
                {
                    Console.WriteLine(wordMetric);
                }

                Console.WriteLine(@"Tmp Metrics");
                foreach (var wordMetric in context.TmpWordMetrics)
                {
                    Console.WriteLine(wordMetric);
                }
            }

            DoBulkMerge();

            Console.WriteLine(@"Complete");

            using (var context = new CountVonCountDbContext())
            {
                foreach (var wordMetric in context.WordMetrics.AsNoTracking().OrderByDescending(w => w.Count).Take(100))
                {
                    Console.WriteLine(wordMetric);
                }
            }

            using (var context = new CountVonCountDbContext())
            {
                var connString = ConfigurationManager.ConnectionStrings["CountVonCountDBConnectionString"]
                    .ConnectionString;
                var persist = new EntityFrameworkRepository<CountVonCountDbContext>(connString, context);

                Expression<Func<WordMetric, bool>> where = wordM => wordM.Count > 10;
                Func<IQueryable<WordMetric>, IOrderedQueryable<WordMetric>> orderBy = entities => entities.OrderByDescending(x => x.Count);
                const int take = 100;

                var results = persist.Get(where, orderBy, take);

                foreach (var result in results)
                {
                    Console.WriteLine($@"repo : {result}");
                }

            }

            Console.ReadLine();
        }

        private static void DoBulkMerge()
        {
            using (var context = new CountVonCountDbContext())
            {
                context.Database.ExecuteSqlCommand("exec p_MergeIntoWordMetrics");
                context.SaveChanges();

            }
        }

        private static void DoBulkInsertUsingExtension()
        {
            Console.WriteLine(@"Do Bulk Insert");
            var entities = CreateWords(1);
            var stopwatch = Stopwatch.StartNew();
            using (var context = new CountVonCountDbContext())
            {
                context.BulkInsert(entities);
            }

            stopwatch.Stop();

            Console.WriteLine($@"Done in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void DoBulkInsertUsingSqlBulkCopy<T>(string tableName) where T : class
        {
            var entities = CreateWords(1000);

            using (var connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["CountVonCountDBConnectionString"].ConnectionString))
            {
                connection.Open();
                using(var transaction = connection.BeginTransaction())
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                using (var reader = ObjectReader.Create(entities, typeof(T).GetProperties().Select(p => p.Name).ToArray()))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(reader);
                    transaction.Commit();
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
