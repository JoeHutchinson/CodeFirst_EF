using CodeFirst_EF.DTOs;
using CodeFirst_EF.Repositories;
using CodeFirst_EF.Tests.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using CodeFirst_EF.Security;
using Moq;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Numbers;
using TddXt.AnyRoot.Strings;

namespace CodeFirst_EF.Tests.IntegrationTests.Repositories
{
    [TestFixture]
    public class EntityFrameworkRepositoryWriteOnlyTests : IntegrationTestSetup
    {
        private readonly List<WordMetric> _expectedWordMetrics = new List<WordMetric>()
        {
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(),Root.Any.String()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(),Root.Any.String()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(),Root.Any.String()),
            new WordMetric(Root.Any.String(), Root.Any.String(), Root.Any.Integer(),Root.Any.String())
        };

        [OneTimeSetUp]
        public override void Init()
        {
            base.Init();
        }

        [Test]
        public void WhenUpsertIsCalledAllEntitiesAreWroteToTheDb()
        {
            var mockHashRepository = new Mock<IHashRepository>();
            mockHashRepository.Setup(h => h.Hash(It.IsAny<IEnumerable<WordMetric>>())).Returns(_expectedWordMetrics);

            using (var context = new CountVonCountTestDbContext())
            {
                var sut = new EntityFrameworkRepository<CountVonCountTestDbContext>(context, mockHashRepository.Object);

                sut.Upsert(_expectedWordMetrics);
            }

            using (var context = new CountVonCountTestDbContext())
            {
                CollectionAssert.AreEqual(_expectedWordMetrics.OrderBy(x => x.Id), 
                    context.WordMetrics.OrderBy(x => x.Id), new WordMetricComparer());
            }
        }

    }
}
