using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using TddXt.AnyRoot.Strings;
using static TddXt.AnyRoot.Root;

namespace CodeFirst_EF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome");

            using (var context = new CountVonCountDBContext())
            {
                foreach (var wordMetric in context.WordMetrics)
                {
                    Console.WriteLine(wordMetric);
                }
            }

            Console.WriteLine("Do Bulk Insert");
            var entities = CreateWords(100);
            var stopwatch = Stopwatch.StartNew();
            using (var context = new CountVonCountDBContext())
            {
                context.BulkInsert(entities, options => { options.BatchSize = 10; });
            }
            stopwatch.Stop();

            Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds}ms");
            Console.ReadLine();
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

    public class CountVonCountDBContext : DbContext
    {
        public CountVonCountDBContext() : base("CountVonCountDBConnectionString")
        {
            Database.SetInitializer(new CountVonCountDbInitializer());
        }

        public DbSet<WordMetric> WordMetrics { get; set; }
    }

    public class CountVonCountDbInitializer : DropCreateDatabaseIfModelChanges<CountVonCountDBContext>
    {
        protected override void Seed(CountVonCountDBContext context)
        {
            // seed the DB
            context.WordMetrics.Add(new WordMetric("hashed1", "Vauxhall", 8));
            context.WordMetrics.Add(new WordMetric("hashed2", "Vauxhall", 5));
            context.WordMetrics.Add(new WordMetric("hashed3", "Bmw", 3));
            
            base.Seed(context);
        }
    }

    public class WordMetric
    {
        public WordMetric() { }
        public WordMetric(string id, string word, int count)
        {
            Id = id;
            Word = word;
            Count = count;
        }

        [Key]
        public string Id { get; set; }

        public string Word { get; set; }

        public int Count { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Word)}: {Word}, {nameof(Count)}: {Count}";
        }
    }
}
