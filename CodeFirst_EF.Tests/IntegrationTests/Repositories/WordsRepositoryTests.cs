using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;
using CodeFirst_EF.Tests.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Numbers;
using TddXt.AnyRoot.Strings;

namespace CodeFirst_EF.Tests.IntegrationTests.Repositories
{
    [TestFixture]
    public class WordsRepositoryTests : IntegrationTestSetup
    {
        private readonly List<WordMetric> _expectedWordMetrics = new List<WordMetric>()
        {
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(), Root.Any.String()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(), Root.Any.String()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(), Root.Any.String()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(), Root.Any.String())
        };

        [OneTimeSetUp]
        public override void Init()
        {
            base.Init();

            using (var context = new CountVonCountTestDbContext())
            {
                context.WordMetrics.AddRange(_expectedWordMetrics);
                context.SaveChanges();
            }
        }

        [Test]
        public void WhenGetIsCalledTheTopNumberOfEntitiesRequestedAreReturned()
        {
            var sut = new WordsRepository();
            const int take = 2;
            var expectedOrder = _expectedWordMetrics.OrderByDescending(w => w.Count).Take(take);

            var words = sut.GetTop<CountVonCountTestDbContext>(2);

            CollectionAssert.AreEqual(expectedOrder, words, new WordMetricComparer());
        }
    }
}
