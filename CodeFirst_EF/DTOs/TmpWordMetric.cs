﻿using CodeFirst_EF.Repositories;
using CodeFirst_EF.Security;
using System.ComponentModel.DataAnnotations;

namespace CodeFirst_EF.DTOs
{
    public sealed class TmpWordMetric : IEntity
    {
        public TmpWordMetric() { }

        public TmpWordMetric(string id, string word, int count, string salt)
        {
            Id = id;
            Word = word;
            Count = count;
            Salt = salt;
        }

        [Key, Hash]
        public string Id { get; set; }

        public int Count { get; set; }

        public string Salt { get; set; }

        [HashKey]
        public string Word { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Count)}: {Count}, {nameof(Word)}: {Word}, {nameof(Salt)}: {Salt}";
        }
    }
}