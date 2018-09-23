using System;
using System.Collections.Generic;
using System.Web.Http;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;

namespace CountVonCount.Controllers
{
    public sealed class WordsController : ApiController
    {
        private readonly IWordRepository _repository;

        public WordsController(IWordRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Words
        public IEnumerable<WordMetric> Get()
        {
            try
            {
                var words = _repository.GetTop(100);

                // Would normally do translation from backend representation to frontend here.
                // For simplicity decided to use the same object throughout

                return words;
            }
            catch (Exception e)
            {
                // Need to add a proper logger, planning to use PostSharp, really like the aspected orientated nature of the library
                Console.WriteLine($@"Failed : {e.Message}");
                throw;
            }
        }
    }
}
