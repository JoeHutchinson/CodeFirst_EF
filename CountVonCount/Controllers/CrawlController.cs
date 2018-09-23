using CodeFirst_EF.Collectors;
using System;
using System.Web.Http;
using CodeFirst_EF.Repositories;

namespace CountVonCount.Controllers
{
    /// <summary>
    /// Endpoint for issuing requests to process more data. Could be from multiple sources,
    /// for larger jobs it could provide crawl status for each.
    /// </summary>
    public sealed class CrawlController : ApiController
    {
        private readonly ICollector _collector;
        private readonly IWordRepository _repository;

        public CrawlController(ICollector collector, IWordRepository repository)
        {
            _collector = collector;
            _repository = repository;
        }

        // POST: api/Crawl
        public IHttpActionResult Post([FromBody]FormData data)
        {
            Console.WriteLine($"POST: data: {data}");

            // I was planning to build a factory over Collectors so you could pass it a location
            // it would figure out if it was webpage, a file etc and give you the appropriate collector
            // to collect and parse the data. Sadly just ran out of time, (wife is looking very angry right now!)

            //
            var words = _collector.Collect(data.url);

            // Save the words to the DB. For performance this will use SqlBulkCopy to a temporary table, I didn't get chance to
            // add a TVP onto it, if I did that would be as fast as we can reasonably go without resorting to elaborate solutions.
            // Once loaded into the temp table an Upsert / blind update occurs from the temporary to the main table in one transaction
            // this will update existing records counts by adding on the count from the temporary table. For any new words not
            // previously known about the record is inserted.
            //
            // The reason why this project is .NET 4.7 rather than Core (which admittedly would have been far easier) is that I wanted
            // to use SQL Always Encrypted feature, if not familiar it encrypts in the application without the app knowing other than
            // the SQL connection having "Always Encrypted = true". This encryption is assymetric and it works nicely with Azure Vault.
            //
            // I additionally decided to go for a EF Code First approach, I'm not a fan of SQL littering the domain as its a maintenance
            // nightmare and just generally can trip you up as you adjust your model. To make Code First approach work with Always
            // Encrypted requires Manual DB Migrations, see CountVonCountMigrateDbInitializer.cs. This is nice as it can do an initial
            // generation of the DB then you have control as a dev how you want to tune your DB, it's close to what I've used in a number
            // of companies and seems to work fairly well.

            // Apologies for the EntityFrameworkRepository, I was having a bit too much fun and started building a completely generic
            // repository capable of handling any entity. I soon realised some poor soul has to review this without me there to explain so
            // reined it in. I'm particularly proud of the Get method on that repo, its very flexible allowing most kind of searches to be
            // specified by the caller rather than necessitating an interface change or giving away the crown jewels (direct DB access).
            _repository.Save(words);

            return Ok(data.url);
        }
    }
}
