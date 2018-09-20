using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using CodeFirst_EF.DbContexts;
using CodeFirst_EF.DTOs;
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
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Welcome");

            using (var context = new CountVonCountDbContext())
            {
                foreach (var wordMetric in context.WordMetrics)
                {
                    Console.WriteLine(wordMetric);
                }
            }

            DoBulkInsertUsingExtension();


            Console.ReadLine();
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

        private void DoBulkInsertUsingSqlBulkCopy()
        {
            using (var connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["CountVonCountDBConnectionString"].ConnectionString))
            {
                var transaction = connection.BeginTransaction();
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    //blk.WriteToServer();
                }
            }
            
        }

        private static IEnumerable<WordMetric> CreateWords(int num)
        {
            for (var i = 0; i < num; i++)
            {
                var word = Any.String(10);
                yield return new WordMetric(word, word, 3);
            }
        }
    }
}
