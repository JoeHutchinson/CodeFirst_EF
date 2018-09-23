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
            var words = _collector.CollectWords(data.url);

            return Ok(data.url);
        }
    }
}
