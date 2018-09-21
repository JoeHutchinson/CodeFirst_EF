﻿using System.ComponentModel.DataAnnotations;
using CodeFirst_EF.Persistence;

namespace CodeFirst_EF.DTOs
{
    public class WordMetric : IEntity
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

        public int Count { get; set; }

        public string Word { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Word)}: {Word}, {nameof(Count)}: {Count}";
        }
    }
}