using System.Collections.Generic;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;

namespace CodeFirst_EF.Security
{
    public sealed class WordSaltCache : ISaltCache
    {
        private readonly Dictionary<string, string> _lookup;

        public WordSaltCache(IRepository storageRepository)
        {
            _lookup = new Dictionary<string, string>();

            var entities = storageRepository.Get<WordMetric>();

            foreach (var entity in entities)
            {
                _lookup.Add(entity.Word, entity.Salt);
            }
        }

        public void Add(string key, string value)
        {
            _lookup.Add(key, value);
        }

        public string Get(string key)
        {
            _lookup.TryGetValue(key, out var value);
            return value;
        }
    }

    public interface ISaltCache
    {
        void Add(string key, string value);
        string Get(string key);
    }

    internal sealed class Salt
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
