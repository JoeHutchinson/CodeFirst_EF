using Castle.Core.Internal;
using CodeFirst_EF.Repositories;
using CodeFirst_EF.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeFirst_EF.Security
{
    public sealed class HashRepository : IHashRepository
    {
        private readonly IHashProvider _hashAlgorithm;
        private readonly ISaltCache _saltCache;
        private readonly bool _hashEnabled;

        public HashRepository(IHashProvider hashAlgorithm, ISaltCache saltCache) : this(hashAlgorithm, saltCache, AppSettings.Get<bool>("HashingEnabled"))
        {}

        internal HashRepository(IHashProvider hashAlgorithm, ISaltCache saltCache, bool hashEnabled)
        {
            _hashAlgorithm = hashAlgorithm;
            _saltCache = saltCache;
            _hashEnabled = hashEnabled;
        }

        /// <summary>
        /// Hash all entities based off <see cref="HashAttribute"/> on property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">Entities to hash</param>
        /// <returns>Hashed entities</returns>
        public IEnumerable<T> Hash<T>(IEnumerable<T> entities) where T : IEntity
        {
            if (!_hashEnabled)
            {
                return entities;
            }

            var type = typeof(T);
            var hashProperties = GetPropertiesToHash(type);

            if (hashProperties.IsNullOrEmpty())
            {
                return entities;
            }

            var hashPropertyKey = GetHashKeyProperty(type);
            var hashedEntities = entities.AsParallel().Select(e =>
            {
                var hashKey = type.GetProperty(hashPropertyKey).GetValue(e).ToString();
                foreach (var propertyName in hashProperties)
                {
                    // Read the unhashed property
                    var unhashed = type.GetProperty(propertyName).GetValue(e).ToString();

                    var existingSalt = _saltCache.Get(hashKey);
                    var hash = _hashAlgorithm.CreateHash(unhashed, existingSalt);

                    // Set the hashed value onto the property and add the salt too
                    type.GetProperty(propertyName).SetValue(e, hash.Hash);
                    type.GetProperty("Salt").SetValue(e, hash.Salt);
                }

                return e;
            });

            // Add salt to cache
            foreach (var entity in hashedEntities)
            {
                var salt = type.GetProperty("Salt").GetValue(entity);
                if (salt != null)
                {
                    _saltCache.TryAdd(entity.Word, salt.ToString());
                }
            }

            return entities;
        }

        /// <summary>
        /// Use reflection to identify any property with Hash custom attribute
        /// </summary>
        /// <param name="t">Entity type</param>
        /// <returns></returns>
        private static IEnumerable<string> GetPropertiesToHash(Type t)
        {
            var properties = t.GetProperties();
            return from property in t.GetProperties()
                where property.GetCustomAttributes().Any(att => att.GetType() == typeof(HashAttribute))
                select property.Name;
        }

        /// <summary>
        /// Use reflection to identify the property that signifies the key to lookup for existing salt
        /// </summary>
        /// <param name="t">Entity type</param>
        /// <returns></returns>
        private static string GetHashKeyProperty(Type t)
        {
            var properties = t.GetProperties();
            return properties.First(p => p.GetCustomAttributes().Any(att => att.GetType() == typeof(HashKeyAttribute)))
                .Name;
        }
    }

    public interface IHashRepository
    {
        IEnumerable<T> Hash<T>(IEnumerable<T> entities) where T : IEntity;
    }
}
