using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CountVonCount.Controllers
{
    /// <summary>
    /// Endpoint for issuing requests to process more data. Could be from multiple sources,
    /// for larger jobs it could provide crawl status for each.
    /// </summary>
    public sealed class CrawlController : ApiController
    {
        // GET: api/Crawl
        public IEnumerable<string> Get()
        {
            return new [] { "value1", "value2" };
        }

        // POST: api/Crawl
        public IHttpActionResult Post([FromBody]FormData data)
        {
            Console.WriteLine($"POST: data: {data}");

            return Ok(data.url);
        }
    }
}
