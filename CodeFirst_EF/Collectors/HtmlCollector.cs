using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using CodeFirst_EF.DTOs;

namespace CodeFirst_EF.Collectors
{
    public sealed class HtmlCollector : ICollector
    {

        private readonly IHtmlProvider _htmlProvider;

        public HtmlCollector(IHtmlProvider htmlProvider)
        {
            _htmlProvider = htmlProvider;
        }

        public IEnumerable<WordMetric> Collect(string url)
        {
            var htmlDoc = GetHtmlDocument(url);

            RemoveDescendents(htmlDoc);

            var visibleText = ExtractVisibleText(htmlDoc);

            var cleanedText = CleanVisibleText(visibleText);

            var countedWords = CountWordOccurances(cleanedText);

            return countedWords;
        }

        internal IEnumerable<WordMetric> CountWordOccurances(string inputText)
        {

            var knownWords = new Dictionary<string, WordMetric>();
            foreach (var word in inputText.ToLower().Split(' '))
            {
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }

                if (knownWords.ContainsKey(word))
                {
                    knownWords[word].Count++;
                }
                else
                {
                    knownWords.Add(word, new WordMetric(word, word, 1, null));
                }
            }

            var result = knownWords.Values.ToList();
            return result;
        }

        internal HtmlDocument GetHtmlDocument(string url)
        {
            var htmlDoc = _htmlProvider.Load(url);
            return htmlDoc;
        }

        /// <summary>
        /// Remove any descendants in the HTML doc not containing visible text
        /// </summary>
        /// <param name="htmlDoc"></param>
        internal static void RemoveDescendents(HtmlDocument htmlDoc)
        {
            foreach (var script in htmlDoc.DocumentNode.Descendants("script").ToArray())
                script.Remove();
            foreach (var style in htmlDoc.DocumentNode.Descendants("style").ToArray())
                style.Remove();
        }

        /// <summary>
        /// Remove characters and symbols not considered words
        /// </summary>
        /// <param name="visibleText">Text to be cleaned</param>
        /// <returns>String with spaces separating words</returns>
        internal static string CleanVisibleText(string visibleText)
        {
            var decoded = WebUtility.HtmlDecode(visibleText);

            var removedUrls = Regex.Replace(decoded, @"http[^\s]+", "");
            var cleanedText = Regex.Replace(removedUrls, @"[^A-Za-z\s]+", "");
            return cleanedText;
        }

        /// <summary>
        /// Traverse all nodes and extract visible text
        /// </summary>
        /// <param name="htmlDoc">Document to traverse</param>
        /// <returns>String</returns>
        internal static string ExtractVisibleText(HtmlDocument htmlDoc)
        {
            var s = new StringBuilder();
            foreach (var node in htmlDoc.DocumentNode.DescendantsAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    var text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        s.Append(text.Trim() + " ");
                }
            }

            return s.ToString().Trim();
        }
    }
}
