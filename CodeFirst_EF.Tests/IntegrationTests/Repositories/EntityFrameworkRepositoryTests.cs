using System;
using System.Collections.Generic;
using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;
using CodeFirst_EF.Tests.Helpers;
using NUnit.Framework;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Numbers;
using TddXt.AnyRoot.Strings;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CodeFirst_EF.Tests.IntegrationTests.Repositories
{
    [TestFixture]
    public class EntityFrameworkRepositoryReadOnlyTests : IntegrationTestSetup
    {
        private readonly List<WordMetric> _expectedWordMetrics = new List<WordMetric>()
        {
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer())
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
        public void WhenGetIsCalledAndNoParametersAreSuppliedThenAllEntitiesAreReturned()
        {
            using (var context = new CountVonCountTestDbContext())
            {
                var connString = ConfigurationManager.ConnectionStrings["CountVonCountTestDBConnectionString"]
                    .ConnectionString;

                var sut = new EntityFrameworkRepository<CountVonCountTestDbContext>(connString, context);

                var entities = sut.Get<WordMetric>();

                Assert.AreEqual(_expectedWordMetrics.Count, entities.Count());
            }
        }

        [Test]
        public void WhenGetIsCalledAndTakeIsSuppliedThenUpToThatNumberOfEntitiesAreReturned()
        {
            using (var context = new CountVonCountTestDbContext())
            {
                var connString = ConfigurationManager.ConnectionStrings["CountVonCountTestDBConnectionString"]
                    .ConnectionString;

                var sut = new EntityFrameworkRepository<CountVonCountTestDbContext>(connString, context);

                const int take = 3;
                var entities = sut.Get<WordMetric>(null, null, take);

                Assert.AreEqual(take, entities.Count());
            }
        }

        [Test]
        public void WhenGetIsCalledAndOrderByIsSuppliedAllEntitiesAreReturnedInOrder()
        {
            using (var context = new CountVonCountTestDbContext())
            {
                var connString = ConfigurationManager.ConnectionStrings["CountVonCountTestDBConnectionString"]
                    .ConnectionString;

                var sut = new EntityFrameworkRepository<CountVonCountTestDbContext>(connString, context);

                IOrderedQueryable<WordMetric> OrderByDesc(IQueryable<WordMetric> words) => words.OrderByDescending(x => x.Count);

                var expectedOrder = OrderByDesc(_expectedWordMetrics.AsQueryable()).ToList();
                var entities = sut.Get<WordMetric>(null, OrderByDesc);

                CollectionAssert.AreEqual(expectedOrder, entities, new WordMetricComparer());
            }
        }

        [Test]
        public void WhenGetIsCalledAndWhereIsSuppliedEntitiesSatisfyingTheClauseAreReturned()
        {
            using (var context = new CountVonCountTestDbContext())
            {
                var connString = ConfigurationManager.ConnectionStrings["CountVonCountTestDBConnectionString"]
                    .ConnectionString;

                var sut = new EntityFrameworkRepository<CountVonCountTestDbContext>(connString, context);

                var secondTopCount = _expectedWordMetrics.OrderByDescending(w => w.Count).ToArray()[1].Count;
                var expectedEntities = _expectedWordMetrics.Where(w => w.Count > secondTopCount);
                Expression<Func<WordMetric, bool>> where = wordM => wordM.Count > secondTopCount;

                var entities = sut.Get(where);

                CollectionAssert.AreEqual(expectedEntities.OrderBy(x => x.Count), entities.OrderBy(x => x.Count),
                    new WordMetricComparer());
            }
        }
    }
}
