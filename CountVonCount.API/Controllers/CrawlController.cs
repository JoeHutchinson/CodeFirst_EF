using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CountVonCount.API.Controllers
{
    /// <summary>
    /// Endpoint for issuing requests to process more data. Could be from multiple sources,
    /// for larger jobs it could provide crawl status for each.
    /// </summary>
    public class CrawlController : ApiController
    {
        // GET: api/Crawl
        public IEnumerable<string> Get()
        {
            return new [] { "value1", "value2" };
        }

        // POST: api/Crawl
        public void Post([FromBody]string value)
        {
            Console.WriteLine($"POST: value: {value}");
        }

        // DELETE: api/Crawl/5
        public void Delete(int id)
        {
        }
    }
}
