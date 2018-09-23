using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CountVonCount.API.Controllers
{
    public class WordsController : ApiController
    {
        private readonly IWordRepository _repository;

        public WordsController(IWordRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Words
        public IEnumerable<WordMetric> Get()
        {
            var words = _repository.GetTop(100);

            // Would normally do translation from backend representation to frontend here.
            // For simplicity decided to use the same object throughout

            return words;
        }

        // GET: api/Words/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Words
        public void Post([FromBody]string value)
        {
            Console.WriteLine($"POST: value: {value}");
        }

        // DELETE: api/Words/5
        public void Delete(int id)
        {
        }
    }
}
